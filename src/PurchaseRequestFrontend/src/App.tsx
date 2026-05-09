import { useEffect, useState } from 'react'

import './App.css'
import { AppShell } from './components/AppShell/AppShell'
import { ApprovalView } from './components/ApprovalView/ApprovalView'
import { RequestDetail } from './components/RequestDetail/RequestDetail'
import { RequestForm } from './components/RequestForm/RequestForm'
import { RequestsList } from './components/RequestsList/RequestsList'
import { deleteRequestApi } from './api'
import type { RequestItemApiDto } from './api'
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
    category: item.unitsOfMeasure,
    quantity: item.quantity,
    unitPrice: item.unitPrice,
    productId: item.productId,
  }))
}

function App() {
  const [requestRecords, setRequestRecords] = useState<RequestRecord[]>([])
  const [screen, setScreen] = useState<Screen>('requests')
  const [selectedId, setSelectedId] = useState('')
  const [filter, setFilter] = useState<'All' | Status>('All')
  const [typeFilter, setTypeFilter] = useState<string>('All')
  const [decision, setDecision] = useState<DecisionState>('idle')
  const [visibleCount, setVisibleCount] = useState(6)

  useEffect(() => {
    async function fetchRequests() {
      try {
        const response = await fetch('/Request')
        const result = await response.json()

        if (result.isSuccess && result.data) {
          setRequestRecords(
            result.data.map((dto: any) => {
              const items = mapApiItems(dto.items)

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
                updated: new Date(dto.updatedAt).toLocaleDateString(),
                submitted: new Date(dto.createdAt).toLocaleDateString(),
                approver: 'Sarah Chen',
                description: dto.description ?? '',
                items,
              }
            }),
          )
        }
      } catch (error) {
        console.error('Failed to load requests:', error)
      }
    }

    fetchRequests()
  }, [])

  const uniqueTypes = [
    'All',
    ...new Set(requestRecords.map((request) => request.type)),
  ]
  const selectedRequest =
    requestRecords.find((request) => request.id === selectedId) ??
    blankRequest
  const allFilteredRequests = requestRecords.filter((request) => {
    const matchesStatus = filter === 'All' || request.status === filter
    const matchesType = typeFilter === 'All' || request.type === typeFilter

    return matchesStatus && matchesType
  })
  const filteredRequests = allFilteredRequests.slice(0, visibleCount)
  const reviewCount = requestRecords.filter((request) =>
    ['New', 'Resubmitted'].includes(request.status),
  ).length

  function openRequest(request: RequestRecord, target: Screen = 'detail') {
    setSelectedId(request.id)
    setDecision('idle')
    setScreen(target)
  }

  function createRequest(request: RequestRecord) {
    setRequestRecords((currentRequests) => [request, ...currentRequests])
    setSelectedId(request.id)
    setFilter('All')
    setTypeFilter('All')
    setVisibleCount(6)
    setDecision('idle')
    setScreen('requests')
  }

  function updateRequest(request: RequestRecord) {
    setRequestRecords((currentRequests) =>
      currentRequests.map((currentRequest) =>
        currentRequest.id === request.id ? request : currentRequest,
      ),
    )
    setSelectedId(request.id)
    setDecision('idle')
    setScreen('detail')
  }

  async function deleteRequest(id: string) {
    const result = await deleteRequestApi(id)

    if (!result.isSuccess) {
      throw new Error(result.error?.message ?? 'Failed to delete request.')
    }

    setRequestRecords((currentRequests) =>
      currentRequests.filter((request) => request.id !== id),
    )
    setSelectedId('')
    setDecision('idle')
    setScreen('requests')
  }

  function decideRequest(
    decisionResult: 'approved' | 'rejected',
    reason = '',
    finalRejected = false,
  ) {
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
      onCreate={() => setScreen('create')}
      onOpen={openRequest}
      onRequests={() => setScreen('requests')}
      requestRecords={requestRecords}
      reviewCount={reviewCount}
      screen={screen}
    >
      {screen === 'requests' && (
        <RequestsList
          filter={filter}
          filteredRequests={filteredRequests}
          onCreate={() => setScreen('create')}
          onFilter={(nextFilter) => {
            setFilter(nextFilter)
            setVisibleCount(6)
          }}
          onOpen={openRequest}
          onShowMore={() => setVisibleCount((count) => count + 6)}
          onTypeFilter={(nextTypeFilter) => {
            setTypeFilter(nextTypeFilter)
            setVisibleCount(6)
          }}
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
          onCancel={() => setScreen('requests')}
          onSubmit={createRequest}
        />
      )}

      {screen === 'edit' && (
        <RequestForm
          mode="edit"
          request={selectedRequest}
          onCancel={() => setScreen('detail')}
          onSubmit={updateRequest}
        />
      )}

      {screen === 'detail' && (
        <RequestDetail
          request={selectedRequest}
          onApprove={() => setScreen('approval')}
          onBack={() => setScreen('requests')}
          onDelete={deleteRequest}
          onEdit={() => setScreen('edit')}
        />
      )}

      {screen === 'approval' && (
        <ApprovalView
          decision={decision}
          onBack={() => setScreen('requests')}
          onDecide={decideRequest}
          request={selectedRequest}
        />
      )}
    </AppShell>
  )
}

export default App
