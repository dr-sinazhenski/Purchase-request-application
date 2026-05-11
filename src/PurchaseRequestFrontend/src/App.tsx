import { useEffect, useState } from 'react'

import './App.css'
import { AppShell } from './components/AppShell/AppShell'
import { ApprovalView } from './components/ApprovalView/ApprovalView'
import { RequestDetail } from './components/RequestDetail/RequestDetail'
import { RequestForm } from './components/RequestForm/RequestForm'
import { RequestsList } from './components/RequestsList/RequestsList'
import { Topbar } from './components/Topbar/Topbar'
import {
  approveRequestApi,
  deleteRequestApi,
  loadRequestDetails,
  loadRequests,
  rejectRequestApi,
} from './api'
import type { RequestDetailsApiDto, RequestItemApiDto } from './api'
import {
  getRouteScreen,
  parseRoute,
  routeToPath,
  type AppRoute,
} from './router'
import type { DecisionState, RequestRecord, Screen, Status } from './types'

const blankRequest: RequestRecord = {
  id: '',
  name: '',
  type: '',
  status: 'New',
  total: 0,
  creator: 'Current user',
  initials: 'CU',
  updated: 'Draft',
  submitted: 'Not submitted yet',
  approver: 'Sarah Chen',
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

function mapApiItems(items: RequestItemApiDto[] = []) {
  return items.map((item) => ({
    name: item.name,
    category: item.description,
    quantity: item.amount,
    unitPrice: item.price,
    productId: item.id,
  }))
}

function mapApiRequest(dto: RequestDetailsApiDto): RequestRecord {
  const items = mapApiItems(dto.products)
  const createdAt = dto.createdAt ? new Date(dto.createdAt) : new Date()
  const updatedAt = dto.updatedAt ? new Date(dto.updatedAt) : createdAt

  return {
    id: dto.id,
    name: dto.title,
    type: dto.requestType.name,
    status: normalizeStatus(dto.status),
    total: items.reduce(
      (sum, item) => sum + item.quantity * item.unitPrice,
      0,
    ),
    creator: 'Current user',
    initials: 'CU',
    updated: updatedAt.toLocaleDateString(),
    submitted: createdAt.toLocaleDateString(),
    approver: 'Sarah Chen',
    description: dto.description ?? '',
    reason: dto.rejectionCommentText,
    finalRejected: dto.status === 'FinalReject',
    items,
  }
}

function App() {
  const [route, setRoute] = useState<AppRoute>(() =>
    parseRoute(window.location.pathname),
  )
  const [requestRecords, setRequestRecords] = useState<RequestRecord[]>([])
  const [filter, setFilter] = useState<'All' | Status>('All')
  const [searchQuery, setSearchQuery] = useState('')
  const [typeFilter, setTypeFilter] = useState<string>('All')
  const [decision, setDecision] = useState<DecisionState>('idle')
  const [visibleCount, setVisibleCount] = useState(6)
  const screen = getRouteScreen(route)
  const selectedId = ('requestId' in route ? route.requestId : '') ?? ''

  useEffect(() => {
    async function fetchRequests() {
      try {
        const result = await loadRequests()

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

          setRequestRecords(details.map(mapApiRequest))
        }
      } catch (error) {
        console.error('Failed to load requests:', error)
      }
    }

    fetchRequests()
  }, [])

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
          const detailedRequest = mapApiRequest(result.data)

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
  }, [selectedId])

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

  const uniqueTypes = [
    'All',
    ...new Set(requestRecords.map((request) => request.type)),
  ]
  const selectedRequest =
    requestRecords.find((request) => request.id === selectedId) ??
    blankRequest
  const selectedApprovalRequest = selectedId
    ? requestRecords.find((request) => request.id === selectedId)
    : undefined
  const normalizedSearchQuery = searchQuery.trim().toLowerCase()
  const allFilteredRequests = requestRecords.filter((request) => {
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
  const reviewCount = requestRecords.filter((request) =>
    ['New', 'Resubmitted'].includes(request.status),
  ).length
  const approvalQueueRequests = requestRecords.filter((request) =>
    ['New', 'Resubmitted'].includes(request.status),
  )

  async function openRequest(request: RequestRecord, target: Screen = 'detail') {
    if (target === 'create' || target === 'requests') {
      navigate({ screen: target })
      return
    }

    navigate({ screen: target, requestId: request.id })
  }

  function createRequest(request: RequestRecord) {
    setRequestRecords((currentRequests) => [request, ...currentRequests])
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
    navigate({ screen: 'requests' })
  }

  async function decideRequest(
    decisionResult: 'approved' | 'rejected',
    reason = '',
    finalRejected = false,
  ) {
    if (!selectedRequest.id) {
      throw new Error('Select a request before recording a decision.')
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

  return (
    <AppShell
      onApprovalQueue={() => navigate({ screen: 'approval' })}
      onCreate={() => navigate({ screen: 'create' })}
      onProfile={() => navigate({ screen: 'profile' })}
      onRequests={() => navigate({ screen: 'requests' })}
      reviewCount={reviewCount}
      screen={screen}
    >
      {screen === 'requests' && (
        <RequestsList
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
          totalRequests={requestRecords.length}
          typeFilter={typeFilter}
          uniqueTypes={uniqueTypes}
          visibleCount={visibleCount}
        />
      )}

      {screen === 'create' && (
        <RequestForm
          mode="create"
          request={blankRequest}
          onCancel={() => navigate({ screen: 'requests' })}
          onSubmit={createRequest}
        />
      )}

      {screen === 'edit' && (
        <RequestForm
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

      {screen === 'approval' && (
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

      {screen === 'profile' && (
        <>
          <Topbar title="Profile" />
          <section className="content-area" />
        </>
      )}
    </AppShell>
  )
}

export default App
