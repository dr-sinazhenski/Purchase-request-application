import { useState } from 'react'

import './App.css'
import { AppShell } from './components/AppShell/AppShell'
import { ApprovalView } from './components/ApprovalView/ApprovalView'
import { RequestDetail } from './components/RequestDetail/RequestDetail'
import { RequestForm } from './components/RequestForm/RequestForm'
import { RequestsList } from './components/RequestsList/RequestsList'
import { draftRequest, initialRequests } from './data/requests'
import type { DecisionState, RequestRecord, Screen, Status } from './types'

function App() {
  const [requestRecords, setRequestRecords] = useState(initialRequests)
  const [screen, setScreen] = useState<Screen>('requests')
  const [selectedId, setSelectedId] = useState('REQ-0042')
  const [filter, setFilter] = useState<'All' | Status>('All')
  const [typeFilter, setTypeFilter] = useState<string>('All')
  const [decision, setDecision] = useState<DecisionState>('idle')
  const [visibleCount, setVisibleCount] = useState(6)

  const uniqueTypes = [
    'All',
    ...new Set(requestRecords.map((request) => request.type)),
  ]
  const selectedRequest =
    requestRecords.find((request) => request.id === selectedId) ??
    requestRecords[0]
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
          request={draftRequest}
          onCancel={() => setScreen('requests')}
          onSubmit={createRequest}
          requestCount={requestRecords.length}
        />
      )}

      {screen === 'edit' && (
        <RequestForm
          mode="edit"
          request={selectedRequest}
          onCancel={() => setScreen('detail')}
          onSubmit={updateRequest}
          requestCount={requestRecords.length}
        />
      )}

      {screen === 'detail' && (
        <RequestDetail
          request={selectedRequest}
          onApprove={() => setScreen('approval')}
          onBack={() => setScreen('requests')}
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
