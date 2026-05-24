import { useEffect, useState } from 'react'

import './App.css'
import { AdminView } from './components/AdminView/AdminView'
import { AppShell } from './components/AppShell/AppShell'
import { ApprovalView } from './components/ApprovalView/ApprovalView'
import { AuthView } from './components/AuthView/AuthView'
import { ProfileView } from './components/ProfileView/ProfileView'
import { RequestDetail } from './components/RequestDetail/RequestDetail'
import { RequestForm } from './components/RequestForm/RequestForm'
import { RequestsList } from './components/RequestsList/RequestsList'
import {
  approveRequestApi,
  deleteRequestApi,
  loadAccounts,
  loadRequestDetails,
  loadRequests,
  loadRequestsFiltered,
  rejectRequestApi,
} from './api'
import type { AccountOption, RequestDetailsApiDto, RequestItemApiDto } from './api'
import {
  getRouteScreen,
  parseRoute,
  routeToPath,
  type AppRoute,
} from './router'
import type { DecisionState, RequestRecord, Screen, Status } from './types'
import { hasRole } from './utils/roles'

const blankRequest: RequestRecord = {
  id: '',
  name: '',
  type: '',
  typeId: undefined,
  status: 'New',
  total: 0,
  creator: 'Current user',
  initials: 'CU',
  updated: 'Draft',
  submitted: 'Not submitted yet',
  approver: 'Approval queue',
  description: '',
  items: [
    {
      productId: undefined,
      name: '',
      category: '',
      quantity: 1,
      unitPrice: 0,
    },
  ],
}

const accountStorageKey = 'procureflow.account'
const requestOwnersStorageKey = 'procureflow.requestOwners'

function getStoredRequestOwners() {
  const storedOwners = window.localStorage.getItem(requestOwnersStorageKey)

  if (!storedOwners) {
    return {}
  }

  try {
    return JSON.parse(storedOwners) as Record<string, string>
  } catch {
    window.localStorage.removeItem(requestOwnersStorageKey)
    return {}
  }
}

function normalizeStatus(status: string): Status {
  switch (status) {
    case 'Submited':
      return 'New'
    case 'Resubmited':
      return 'Resubmitted'
    case 'Approved':
      return 'Approved'
    case 'Rejected':
    case 'FinalReject':
      return 'Rejected'
    default:
      return 'New'
  }
}

function mapUiStatusToBackend(status: 'All' | Status) {
  switch (status) {
    case 'New':
      return 'Submited'
    case 'Resubmitted':
      return 'Resubmited'
    case 'Approved':
      return 'Approved'
    case 'Rejected':
      return 'Rejected'
    default:
      return undefined
  }
}

function mapApiItems(items: RequestItemApiDto[] = []) {
  return items.map((item) => ({
    name: item.name,
    category: item.description,
    quantity: item.amount,
    unitPrice: item.price,
    productId: item.id,
  }))
}

function getInitials(name = '') {
  return (
    name
      .split(' ')
      .filter(Boolean)
      .map((part) => part[0])
      .join('')
      .slice(0, 2)
      .toUpperCase() || 'CU'
  )
}

function mapApiRequest(
  dto: RequestDetailsApiDto,
  metadata: {
    approverName?: string
    creatorName?: string
    ownerAccountId?: string
  } = {},
): RequestRecord {
  const items = mapApiItems(dto.products)
  const createdAt = dto.createdAt ? new Date(dto.createdAt) : new Date()
  const updatedAt = dto.updatedAt ? new Date(dto.updatedAt) : createdAt
  const creator = metadata.creatorName ?? 'Backend request'

  return {
    id: dto.id,
    name: dto.title,
    type: dto.requestType.name,
    typeId: dto.requestType.id,
    status: normalizeStatus(dto.status),
    total: items.reduce(
      (sum, item) => sum + item.quantity * item.unitPrice,
      0,
    ),
    creator,
    initials: getInitials(creator),
    updated: updatedAt.toLocaleDateString(),
    submitted: createdAt.toLocaleDateString(),
    approver: metadata.approverName ?? 'Approval queue',
    description: dto.description ?? '',
    reason: dto.rejectionCommentText,
    finalRejected: dto.status === 'FinalReject',
    items,
    ownerAccountId: metadata.ownerAccountId,
  }
}

function App() {
  const [route, setRoute] = useState<AppRoute>(() =>
    parseRoute(window.location.pathname),
  )
  const [currentAccount, setCurrentAccount] = useState<AccountOption | undefined>(
    () => {
      const storedAccount = window.localStorage.getItem(accountStorageKey)

      if (!storedAccount) {
        return undefined
      }

      try {
        return JSON.parse(storedAccount) as AccountOption
      } catch {
        window.localStorage.removeItem(accountStorageKey)
        return undefined
      }
    },
  )
  const [requestOwners, setRequestOwners] = useState<Record<string, string>>(
    () => getStoredRequestOwners(),
  )
  const [accounts, setAccounts] = useState<AccountOption[]>([])
  const [requestTypeIdsByName, setRequestTypeIdsByName] = useState<
    Record<string, string>
  >({})
  const [requestRecords, setRequestRecords] = useState<RequestRecord[]>([])
  const [filter, setFilter] = useState<'All' | Status>('All')
  const [searchQuery, setSearchQuery] = useState('')
  const [typeFilter, setTypeFilter] = useState<string>('All')
  const [decision, setDecision] = useState<DecisionState>('idle')
  const [visibleCount, setVisibleCount] = useState(6)
  const screen = getRouteScreen(route)
  const selectedId = ('requestId' in route ? route.requestId : '') ?? ''
  const canReviewRequests =
    hasRole(currentAccount, 'Approver') || hasRole(currentAccount, 'Admin')
  const canManageAdmin = hasRole(currentAccount, 'Admin')
  const isRequester =
    hasRole(currentAccount, 'Requester') &&
    !hasRole(currentAccount, 'Approver') &&
    !canManageAdmin
  const canCreateRequests = isRequester || canManageAdmin
  const selectedRequestTypeId =
    typeFilter === 'All' ? undefined : requestTypeIdsByName[typeFilter]

  useEffect(() => {
    if (!currentAccount?.id) {
      return
    }

    const accountId = currentAccount.id

    async function refreshCurrentAccount() {
      try {
        const result = await loadAccounts()

        if (result.isSuccess && result.data) {
          setAccounts(result.data)
          const account = result.data.find(
            (option) => option.id === accountId,
          )

          if (account) {
            setCurrentAccount(account)
            window.localStorage.setItem(accountStorageKey, JSON.stringify(account))
          }
        }
      } catch (error) {
        console.error('Failed to refresh current account:', error)
      }
    }

    refreshCurrentAccount()
  }, [currentAccount?.id])

  useEffect(() => {
    if (currentAccount || screen === 'signin' || screen === 'signup') {
      return
    }

    async function loadKnownAccounts() {
      try {
        const result = await loadAccounts()

        if (result.isSuccess && result.data) {
          setAccounts(result.data)
        }
      } catch (error) {
        console.error('Failed to load accounts:', error)
      }
    }

    loadKnownAccounts()
  }, [currentAccount, screen])

  useEffect(() => {
    if (screen === 'signin' || screen === 'signup') {
      return
    }

    async function fetchRequests() {
      try {
        const shouldUseBackendFilter =
          filter !== 'All' || Boolean(selectedRequestTypeId)
        const result = shouldUseBackendFilter
          ? await loadRequestsFiltered({
              regionId: currentAccount?.regionId,
              requestTypeId: selectedRequestTypeId,
              status: mapUiStatusToBackend(filter),
            })
          : await loadRequests()

        if (result.isSuccess && result.data) {
          const detailResults = await Promise.all(
            result.data.map((dto) => loadRequestDetails(dto.id)),
          )
          const details = detailResults
            .filter(
              (
                detailResult,
              ): detailResult is {
                isSuccess: true
                data: RequestDetailsApiDto
              } => Boolean(detailResult.isSuccess && detailResult.data),
            )
            .map((detailResult) => detailResult.data)

          setRequestTypeIdsByName((currentTypes) => {
            const nextTypes = { ...currentTypes }

            details.forEach((detail) => {
              nextTypes[detail.requestType.name] = detail.requestType.id
            })

            return nextTypes
          })

          setRequestRecords(
            details.map((detail) => {
              const ownerAccountId = requestOwners[detail.id]
              const ownerAccount = accounts.find(
                (account) => account.id === ownerAccountId,
              )
              const request = mapApiRequest(detail, {
                approverName:
                  currentAccount?.approverProfileName ?? 'Approval queue',
                creatorName: ownerAccount?.name,
                ownerAccountId,
              })

              return {
                ...request,
              }
            }),
          )
        }
      } catch (error) {
        console.error('Failed to load requests:', error)
      }
    }

    fetchRequests()
  }, [
    accounts,
    currentAccount?.approverProfileName,
    currentAccount?.regionId,
    filter,
    requestOwners,
    selectedRequestTypeId,
    screen,
    typeFilter,
  ])

  useEffect(() => {
    function handlePopState() {
      setDecision('idle')
      setRoute(parseRoute(window.location.pathname))
    }

    window.addEventListener('popstate', handlePopState)

    return () => {
      window.removeEventListener('popstate', handlePopState)
    }
  }, [])

  useEffect(() => {
    if (!selectedId) {
      return
    }

    async function fetchSelectedRequest() {
      try {
        const result = await loadRequestDetails(selectedId)

        if (result.isSuccess && result.data) {
          const ownerAccountId = requestOwners[result.data.id]
          const ownerAccount = accounts.find(
            (account) => account.id === ownerAccountId,
          )
          const request = mapApiRequest(result.data, {
            approverName: currentAccount?.approverProfileName ?? 'Approval queue',
            creatorName: ownerAccount?.name,
            ownerAccountId,
          })
          const detailedRequest = {
            ...request,
          }

          setRequestRecords((currentRequests) => {
            const existingRequest = currentRequests.some(
              (request) => request.id === detailedRequest.id,
            )

            if (!existingRequest) {
              return [detailedRequest, ...currentRequests]
            }

            return currentRequests.map((currentRequest) =>
              currentRequest.id === detailedRequest.id
                ? detailedRequest
                : currentRequest,
            )
          })
        }
      } catch (error) {
        console.error('Failed to load request details:', error)
      }
    }

    fetchSelectedRequest()
  }, [accounts, currentAccount?.approverProfileName, requestOwners, selectedId])

  function navigate(route: AppRoute, replace = false) {
    const path = routeToPath(route)
    const currentPath = `${window.location.pathname}${window.location.search}`

    if (currentPath !== path) {
      if (replace) {
        window.history.replaceState(null, '', path)
      } else {
        window.history.pushState(null, '', path)
      }
    }

    setDecision('idle')
    setRoute(route)
  }

  const roleVisibleRequests =
    isRequester && !canReviewRequests
      ? requestRecords.filter(
          (request) => request.ownerAccountId === currentAccount?.id,
        )
      : requestRecords
  const selectedRequest =
    roleVisibleRequests.find((request) => request.id === selectedId) ??
    blankRequest
  const selectedApprovalRequest =
    selectedId && canReviewRequests
      ? requestRecords.find((request) => request.id === selectedId)
      : undefined
  const uniqueTypes = [
    'All',
    ...new Set([
      ...Object.keys(requestTypeIdsByName),
      ...roleVisibleRequests.map((request) => request.type),
    ]),
  ]
  const normalizedSearchQuery = searchQuery.trim().toLowerCase()
  const allFilteredRequests = roleVisibleRequests.filter((request) => {
    const matchesStatus = filter === 'All' || request.status === filter
    const matchesType = typeFilter === 'All' || request.type === typeFilter
    const searchableText = [
      request.id,
      request.name,
      request.type,
      request.status,
      request.creator,
      request.description,
      request.total.toString(),
      ...request.items.flatMap((item) => [
        item.name,
        item.category,
        item.quantity.toString(),
        item.unitPrice.toString(),
      ]),
    ]
      .join(' ')
      .toLowerCase()
    const matchesSearch =
      normalizedSearchQuery === '' ||
      searchableText.includes(normalizedSearchQuery)

    return matchesStatus && matchesType && matchesSearch
  })
  const filteredRequests = allFilteredRequests.slice(0, visibleCount)
  const approvalQueueRequests = canReviewRequests
    ? requestRecords.filter((request) =>
        ['New', 'Resubmitted'].includes(request.status),
      )
    : []
  const reviewCount = approvalQueueRequests.length

  async function openRequest(request: RequestRecord, target: Screen = 'detail') {
    if (target === 'create' || target === 'requests') {
      navigate({ screen: target })
      return
    }

    if (target === 'approval' && !canReviewRequests) {
      navigate({ screen: 'detail', requestId: request.id })
      return
    }

    if (target === 'detail' || target === 'edit' || target === 'approval') {
      navigate({ screen: target, requestId: request.id })
    }
  }

  function createRequest(request: RequestRecord) {
    const ownedRequest = {
      ...request,
      creator: currentAccount?.name ?? request.creator,
      initials:
        currentAccount?.name ? getInitials(currentAccount.name) : request.initials,
      ownerAccountId: currentAccount?.id,
    }

    if (currentAccount?.id) {
      setRequestOwners((currentOwners) => {
        const nextOwners = {
          ...currentOwners,
          [ownedRequest.id]: currentAccount.id,
        }

        window.localStorage.setItem(
          requestOwnersStorageKey,
          JSON.stringify(nextOwners),
        )

        return nextOwners
      })
    }

    setRequestRecords((currentRequests) => [ownedRequest, ...currentRequests])
    setFilter('All')
    setTypeFilter('All')
    setVisibleCount(6)
    navigate({ screen: 'requests' })
  }

  function updateRequest(request: RequestRecord) {
    setRequestRecords((currentRequests) =>
      currentRequests.map((currentRequest) =>
        currentRequest.id === request.id ? request : currentRequest,
      ),
    )
    navigate({ screen: 'detail', requestId: request.id }, true)
  }

  async function deleteRequest(id: string) {
    const result = await deleteRequestApi(id)

    if (!result.isSuccess) {
      throw new Error(result.error?.message ?? 'Failed to delete request.')
    }

    setRequestRecords((currentRequests) =>
      currentRequests.filter((request) => request.id !== id),
    )
    setRequestOwners((currentOwners) => {
      const nextOwners = { ...currentOwners }
      delete nextOwners[id]
      window.localStorage.setItem(
        requestOwnersStorageKey,
        JSON.stringify(nextOwners),
      )
      return nextOwners
    })
    navigate({ screen: 'requests' })
  }

  function completeAuth(account: AccountOption) {
    setCurrentAccount(account)
    window.localStorage.setItem(accountStorageKey, JSON.stringify(account))
    navigate({ screen: 'requests' })
  }

  function logout() {
    setCurrentAccount(undefined)
    window.localStorage.removeItem(accountStorageKey)
    navigate({ screen: 'signin' })
  }

  function updateCurrentAccount(account: AccountOption) {
    setCurrentAccount(account)
    setAccounts((currentAccounts) => {
      const exists = currentAccounts.some(
        (currentAccountOption) => currentAccountOption.id === account.id,
      )

      return exists
        ? currentAccounts.map((currentAccountOption) =>
            currentAccountOption.id === account.id
              ? account
              : currentAccountOption,
          )
        : [...currentAccounts, account]
    })
    window.localStorage.setItem(accountStorageKey, JSON.stringify(account))
  }

  async function decideRequest(
    decisionResult: 'approved' | 'rejected',
    reason = '',
    finalRejected = false,
  ) {
    if (!selectedRequest.id) {
      throw new Error('Select a request before recording a decision.')
    }

    if (!canReviewRequests) {
      throw new Error('Only approvers can approve or reject requests.')
    }

    const result =
      decisionResult === 'approved'
        ? await approveRequestApi(selectedRequest.id)
        : await rejectRequestApi({
            id: selectedRequest.id,
            reason,
            isFinal: finalRejected,
          })

    if (!result.isSuccess) {
      throw new Error(
        result.error?.message ??
          `Failed to ${decisionResult === 'approved' ? 'approve' : 'reject'} request.`,
      )
    }

    const nextStatus = decisionResult === 'approved' ? 'Approved' : 'Rejected'

    setRequestRecords((currentRequests) =>
      currentRequests.map((request) =>
        request.id === selectedRequest.id
          ? {
              ...request,
              status: nextStatus,
              reason: decisionResult === 'rejected' ? reason : request.reason,
              finalRejected:
                decisionResult === 'rejected' ? finalRejected : undefined,
              updated: 'Just now',
            }
          : request,
      ),
    )
    setDecision(decisionResult)
  }

  if (screen === 'signin' || screen === 'signup') {
    return (
      <AuthView
        mode={screen}
        onModeChange={(nextMode) => navigate({ screen: nextMode })}
        onSuccess={completeAuth}
      />
    )
  }

  return (
    <AppShell
      account={currentAccount}
      canCreateRequests={canCreateRequests}
      canManageAdmin={canManageAdmin}
      canReviewRequests={canReviewRequests}
      onAdmin={() => navigate({ screen: 'admin' })}
      onApprovalQueue={() => navigate({ screen: 'approval' })}
      onCreate={() => navigate({ screen: 'create' })}
      onProfile={() => navigate({ screen: 'profile' })}
      onRequests={() => navigate({ screen: 'requests' })}
      reviewCount={reviewCount}
      screen={screen}
    >
      {screen === 'requests' && (
        <RequestsList
          canCreateRequests={canCreateRequests}
          canReviewRequests={canReviewRequests}
          filter={filter}
          filteredRequests={filteredRequests}
          onCreate={() => navigate({ screen: 'create' })}
          onFilter={(nextFilter) => {
            setFilter(nextFilter)
            setVisibleCount(6)
          }}
          onOpen={openRequest}
          onSearch={(nextSearchQuery) => {
            setSearchQuery(nextSearchQuery)
            setVisibleCount(6)
          }}
          onShowMore={() => setVisibleCount((count) => count + 6)}
          onTypeFilter={(nextTypeFilter) => {
            setTypeFilter(nextTypeFilter)
            setVisibleCount(6)
          }}
          searchQuery={searchQuery}
          totalFiltered={allFilteredRequests.length}
          totalRequests={roleVisibleRequests.length}
          typeFilter={typeFilter}
          uniqueTypes={uniqueTypes}
          visibleCount={visibleCount}
        />
      )}

      {screen === 'create' && (
        canCreateRequests ? (
          <RequestForm
            currentAccount={currentAccount}
            mode="create"
            request={blankRequest}
            onCancel={() => navigate({ screen: 'requests' })}
            onSubmit={createRequest}
          />
        ) : (
          <RequestsList
            canCreateRequests={canCreateRequests}
            canReviewRequests={canReviewRequests}
            filter={filter}
            filteredRequests={filteredRequests}
            onCreate={() => navigate({ screen: 'create' })}
            onFilter={(nextFilter) => {
              setFilter(nextFilter)
              setVisibleCount(6)
            }}
            onOpen={openRequest}
            onSearch={(nextSearchQuery) => {
              setSearchQuery(nextSearchQuery)
              setVisibleCount(6)
            }}
            onShowMore={() => setVisibleCount((count) => count + 6)}
            onTypeFilter={(nextTypeFilter) => {
              setTypeFilter(nextTypeFilter)
              setVisibleCount(6)
            }}
            searchQuery={searchQuery}
            totalFiltered={allFilteredRequests.length}
            totalRequests={roleVisibleRequests.length}
            typeFilter={typeFilter}
            uniqueTypes={uniqueTypes}
            visibleCount={visibleCount}
          />
        )
      )}

      {screen === 'edit' && (
        <RequestForm
          currentAccount={currentAccount}
          mode="edit"
          request={selectedRequest}
          onCancel={() =>
            navigate({ screen: 'detail', requestId: selectedRequest.id })
          }
          onSubmit={updateRequest}
        />
      )}

      {screen === 'detail' && (
        <RequestDetail
          canDelete={
            hasRole(currentAccount, 'Admin') ||
            selectedRequest.ownerAccountId === currentAccount?.id
          }
          canEdit={
            hasRole(currentAccount, 'Admin') ||
            selectedRequest.ownerAccountId === currentAccount?.id
          }
          canReview={canReviewRequests}
          request={selectedRequest}
          onApprove={() =>
            navigate({ screen: 'approval', requestId: selectedRequest.id })
          }
          onBack={() => navigate({ screen: 'requests' })}
          onDelete={deleteRequest}
          onEdit={() =>
            navigate({ screen: 'edit', requestId: selectedRequest.id })
          }
        />
      )}

      {screen === 'approval' && canReviewRequests && (
        <ApprovalView
          decision={decision}
          onBack={() => navigate({ screen: 'approval' })}
          onDecide={decideRequest}
          onOpenRequest={(request) =>
            navigate({ screen: 'approval', requestId: request.id })
          }
          request={selectedApprovalRequest}
          requests={approvalQueueRequests}
        />
      )}

      {screen === 'approval' && !canReviewRequests && (
        <RequestsList
          canCreateRequests={canCreateRequests}
          canReviewRequests={canReviewRequests}
          filter={filter}
          filteredRequests={filteredRequests}
          onCreate={() => navigate({ screen: 'create' })}
          onFilter={(nextFilter) => {
            setFilter(nextFilter)
            setVisibleCount(6)
          }}
          onOpen={openRequest}
          onSearch={(nextSearchQuery) => {
            setSearchQuery(nextSearchQuery)
            setVisibleCount(6)
          }}
          onShowMore={() => setVisibleCount((count) => count + 6)}
          onTypeFilter={(nextTypeFilter) => {
            setTypeFilter(nextTypeFilter)
            setVisibleCount(6)
          }}
          searchQuery={searchQuery}
          totalFiltered={allFilteredRequests.length}
          totalRequests={roleVisibleRequests.length}
          typeFilter={typeFilter}
          uniqueTypes={uniqueTypes}
          visibleCount={visibleCount}
        />
      )}

      {screen === 'profile' && (
        <ProfileView
          account={currentAccount}
          canManageRoles={canManageAdmin}
          onAccountChange={updateCurrentAccount}
          onLogout={logout}
        />
      )}

      {(screen === 'admin' || screen === 'adminUser') && canManageAdmin && (
        <AdminView
          onBackToUsers={() => navigate({ screen: 'admin' })}
          onOpenUser={(userId) => navigate({ screen: 'adminUser', userId })}
          selectedUserId={'userId' in route ? route.userId : undefined}
        />
      )}

      {(screen === 'admin' || screen === 'adminUser') && !canManageAdmin && (
        <RequestsList
          canCreateRequests={canCreateRequests}
          canReviewRequests={canReviewRequests}
          filter={filter}
          filteredRequests={filteredRequests}
          onCreate={() => navigate({ screen: 'create' })}
          onFilter={(nextFilter) => {
            setFilter(nextFilter)
            setVisibleCount(6)
          }}
          onOpen={openRequest}
          onSearch={(nextSearchQuery) => {
            setSearchQuery(nextSearchQuery)
            setVisibleCount(6)
          }}
          onShowMore={() => setVisibleCount((count) => count + 6)}
          onTypeFilter={(nextTypeFilter) => {
            setTypeFilter(nextTypeFilter)
            setVisibleCount(6)
          }}
          searchQuery={searchQuery}
          totalFiltered={allFilteredRequests.length}
          totalRequests={roleVisibleRequests.length}
          typeFilter={typeFilter}
          uniqueTypes={uniqueTypes}
          visibleCount={visibleCount}
        />
      )}
    </AppShell>
  )
}

export default App
