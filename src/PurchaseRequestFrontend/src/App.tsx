import {
  AlertTriangle,
  Check,
  ClipboardCheck,
  FilePenLine,
  FileText,
  Grid2X2,
  Layers3,
  PackagePlus,
  Plus,
  Search,
  Send,
  ShieldCheck,
  SlidersHorizontal,
  X,
} from 'lucide-react'
import { useState } from 'react'
import './App.css'

type Status = 'New' | 'Resubmitted' | 'Approved' | 'Rejected'
type Screen = 'requests' | 'create' | 'detail' | 'edit' | 'approval'

type LineItem = {
  name: string
  category: string
  quantity: number
  unitPrice: number
}

type RequestRecord = {
  id: string
  name: string
  type: string
  status: Status
  total: number
  creator: string
  initials: string
  updated: string
  description: string
  approver: string
  submitted: string
  reason?: string
  finalRejected?: boolean
  items: LineItem[]
}

const statusFilters: Array<'All' | Status> = [
  'All',
  'New',
  'Resubmitted',
  'Approved',
  'Rejected',
]

const initialRequests: RequestRecord[] = [
  {
    id: 'REQ-0042',
    name: 'MacBook Pro 16" - Dev Team',
    type: 'Hardware',
    status: 'Resubmitted',
    total: 3499,
    creator: 'James Kim',
    initials: 'JK',
    updated: '2h ago',
    submitted: 'Dec 14, 2024 at 09:42',
    approver: 'Sarah Chen',
    description:
      'Replacing 2019 MBP for lead developer. Needed for faster Xcode build times and local mobile testing.',
    items: [
      {
        name: 'MacBook Pro 16"',
        category: 'Laptop',
        quantity: 1,
        unitPrice: 3299,
      },
      {
        name: 'AppleCare+',
        category: 'Warranty',
        quantity: 1,
        unitPrice: 200,
      },
    ],
  },
  {
    id: 'REQ-0041',
    name: 'Adobe Creative Suite',
    type: 'Software',
    status: 'Approved',
    total: 6240,
    creator: 'Lisa Park',
    initials: 'LP',
    updated: 'Yesterday',
    submitted: 'Dec 13, 2024 at 16:20',
    approver: 'Sarah Chen',
    description: 'Marketing team annual license for 12 seats.',
    items: [
      {
        name: 'Creative Cloud All Apps',
        category: 'License',
        quantity: 12,
        unitPrice: 520,
      },
    ],
  },
  {
    id: 'REQ-0040',
    name: 'AWS Reserved Instances - Q2',
    type: 'Cloud',
    status: 'New',
    total: 18500,
    creator: 'Marco Rossi',
    initials: 'MR',
    updated: '3 days ago',
    submitted: 'Dec 11, 2024 at 11:10',
    approver: 'Sarah Chen',
    description: 'Q2 infrastructure capacity reservation.',
    items: [
      {
        name: 'Compute reservation',
        category: 'Infrastructure',
        quantity: 1,
        unitPrice: 18500,
      },
    ],
  },
  {
    id: 'REQ-0039',
    name: 'Standing Desk Uplift V2 x 4',
    type: 'Furniture',
    status: 'Rejected',
    total: 2800,
    creator: 'Amy Nguyen',
    initials: 'AN',
    updated: '5 days ago',
    submitted: 'Dec 9, 2024 at 13:45',
    approver: 'Sarah Chen',
    description: 'Office upgrade for new hires joining Q1.',
    reason: 'Budget owner asked to bundle this with the Q1 office refresh.',
    items: [
      {
        name: 'Uplift V2 standing desk',
        category: 'Furniture',
        quantity: 4,
        unitPrice: 700,
      },
    ],
  },
  {
    id: 'REQ-0038',
    name: 'GitHub Enterprise - Annual',
    type: 'Software',
    status: 'Approved',
    total: 21000,
    creator: 'Tom Walsh',
    initials: 'TW',
    updated: '1 week ago',
    submitted: 'Dec 6, 2024 at 10:25',
    approver: 'Sarah Chen',
    description: 'Annual renewal for engineering organization.',
    items: [
      {
        name: 'GitHub Enterprise seats',
        category: 'License',
        quantity: 70,
        unitPrice: 300,
      },
    ],
  },
  {
    id: 'REQ-0037',
    name: 'Figma Organization Plan',
    type: 'Software',
    status: 'New',
    total: 1680,
    creator: 'Diana Lee',
    initials: 'DL',
    updated: '2 weeks ago',
    submitted: 'Dec 1, 2024 at 12:05',
    approver: 'Sarah Chen',
    description: 'Design organization plan for product squads.',
    items: [
      {
        name: 'Figma organization license',
        category: 'License',
        quantity: 8,
        unitPrice: 210,
      },
    ],
  },
]

const draftRequest: RequestRecord = {
  id: 'REQ-0043',
  name: 'Design team hardware refresh',
  type: 'Hardware',
  status: 'New',
  total: 4198,
  creator: 'Nick Sinazhenski',
  initials: 'NS',
  updated: 'Draft',
  submitted: 'Not submitted yet',
  approver: 'Sarah Chen',
  description:
    'Required for the new designer joining in January. Previous request was revised to better align with Q4 budget constraints.',
  items: [
    {
      name: 'MacBook Air 15"',
      category: 'Laptop',
      quantity: 2,
      unitPrice: 1799,
    },
    {
      name: 'USB-C display dock',
      category: 'Accessory',
      quantity: 2,
      unitPrice: 300,
    },
  ],
}

function formatMoney(value: number) {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'EUR',
  }).format(value)
}

function createRequestId(count: number) {
  return `REQ-${String(37 + count).padStart(4, '0')}`
}

function formatSubmittedDate(date: Date) {
  return new Intl.DateTimeFormat('en-US', {
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    month: 'short',
    year: 'numeric',
  }).format(date)
}

function App() {
  const [requestRecords, setRequestRecords] = useState(initialRequests)
  const [screen, setScreen] = useState<Screen>('requests')
  const [selectedId, setSelectedId] = useState('REQ-0042')
  const [filter, setFilter] = useState<'All' | Status>('All')
  const [decision, setDecision] = useState<'idle' | 'approved' | 'rejected'>(
    'idle',
  )

  const selectedRequest =
    requestRecords.find((request) => request.id === selectedId) ??
    requestRecords[0]
  const filteredRequests =
    filter === 'All'
      ? requestRecords
      : requestRecords.filter((request) => request.status === filter)
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
    <div className="app-page">
      <div className="screen-label">ProcureFlow - request workflow demo</div>
      <div className="app-shell">
        <aside className="sidebar">
          <div className="sidebar-header">
            <div className="brand-mark">
              <PackagePlus size={16} strokeWidth={2.4} />
            </div>
            <span>ProcureFlow</span>
          </div>

          <nav className="nav">
            <p className="nav-kicker">Overview</p>
            <button
              className={screen === 'requests' ? 'nav-item active' : 'nav-item'}
              onClick={() => setScreen('requests')}
              type="button"
            >
              <Grid2X2 size={16} />
              All Requests
            </button>
            <button
              className={screen === 'approval' ? 'nav-item active' : 'nav-item'}
              onClick={() => {
                const nextPending =
                  requestRecords.find((request) =>
                    ['New', 'Resubmitted'].includes(request.status),
                  ) ?? requestRecords[0]
                openRequest(nextPending, 'approval')
              }}
              type="button"
            >
              <ClipboardCheck size={16} />
              Approval Queue
              <span className="nav-count">{reviewCount}</span>
            </button>
            <button
              className={screen === 'create' ? 'nav-item active' : 'nav-item'}
              onClick={() => setScreen('create')}
              type="button"
            >
              <FilePenLine size={16} />
              New Request
            </button>
          </nav>

          <div className="sidebar-footer">
            <div className="avatar">SC</div>
            <div>
              <strong>Sarah Chen</strong>
              <span>Approver · Acme Corp</span>
            </div>
          </div>
        </aside>

        <main className="main">
          {screen === 'requests' && (
            <RequestsList
              filter={filter}
              filteredRequests={filteredRequests}
              totalRequests={requestRecords.length}
              onCreate={() => setScreen('create')}
              onFilter={setFilter}
              onOpen={openRequest}
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
        </main>
      </div>
    </div>
  )
}

type RequestsListProps = {
  filter: 'All' | Status
  filteredRequests: RequestRecord[]
  totalRequests: number
  onCreate: () => void
  onFilter: (filter: 'All' | Status) => void
  onOpen: (request: RequestRecord, target?: Screen) => void
}

function RequestsList({
  filter,
  filteredRequests,
  totalRequests,
  onCreate,
  onFilter,
  onOpen,
}: RequestsListProps) {
  return (
    <>
      <Topbar
        count={`${totalRequests} total`}
        primaryAction="+ New Request"
        title="All Requests"
        onPrimary={onCreate}
      />

      <section className="filter-bar">
        <span>Status:</span>
        {statusFilters.map((item) => (
          <button
            className={filter === item ? 'chip active' : 'chip'}
            key={item}
            onClick={() => onFilter(item)}
            type="button"
          >
            {item}
          </button>
        ))}
        <span className="divider" />
        <span>Type:</span>
        <button className="chip" type="button">
          Hardware
        </button>
        <button className="chip" type="button">
          Software
        </button>
        <button className="sort-button" type="button">
          <SlidersHorizontal size={14} />
          Sort: Newest first
        </button>
      </section>

      <section className="content-area">
        <div className="table-card">
          <table>
            <thead>
              <tr>
                <th>Request name ↑</th>
                <th>Type</th>
                <th>Status</th>
                <th>Total price</th>
                <th>Creator</th>
                <th>Last updated</th>
                <th />
              </tr>
            </thead>
            <tbody>
              {filteredRequests.map((request) => (
                <tr key={request.id}>
                  <td>
                    <button
                      className="name-button"
                      onClick={() => onOpen(request)}
                      type="button"
                    >
                      {request.name}
                    </button>
                  </td>
                  <td>{request.type}</td>
                  <td>
                    <StatusBadge
                      finalRejected={request.finalRejected}
                      status={request.status}
                    />
                  </td>
                  <td className="money">{formatMoney(request.total)}</td>
                  <td>
                    <span className="creator">
                      <span className="creator-dot">{request.initials}</span>
                      {request.creator}
                    </span>
                  </td>
                  <td className="muted">{request.updated}</td>
                  <td>
                    <button
                      className="btn compact"
                      onClick={() =>
                        onOpen(
                          request,
                          request.status === 'New' ||
                            request.status === 'Resubmitted'
                            ? 'approval'
                            : 'detail',
                        )
                      }
                      type="button"
                    >
                      {request.status === 'New' ||
                      request.status === 'Resubmitted'
                        ? 'Review'
                        : 'View'}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <div className="table-footer">
          <span>
            Showing {filteredRequests.length} of {totalRequests} requests
          </span>
          <div>
            <button className="btn compact" type="button">
              ← Prev
            </button>
            <button className="btn compact primary" type="button">
              1
            </button>
            <button className="btn compact" type="button">
              2
            </button>
            <button className="btn compact" type="button">
              3
            </button>
            <button className="btn compact" type="button">
              Next →
            </button>
          </div>
        </div>
      </section>
    </>
  )
}

type TopbarProps = {
  count?: string
  primaryAction?: string
  title: string
  onPrimary?: () => void
}

function Topbar({ count, onPrimary, primaryAction, title }: TopbarProps) {
  return (
    <header className="topbar">
      <div>
        <h1>{title}</h1>
        {count && <span>· {count}</span>}
      </div>
      <div className="topbar-actions">
        <label className="search">
          <Search size={14} />
          <input defaultValue="MacBook" aria-label="Search requests" />
        </label>
        <button className="btn" type="button">
          Filter
        </button>
        {primaryAction && (
          <button className="btn primary" onClick={onPrimary} type="button">
            {primaryAction}
          </button>
        )}
      </div>
    </header>
  )
}

type RequestFormProps = {
  mode: 'create' | 'edit'
  request: RequestRecord
  onCancel: () => void
  onSubmit: (request: RequestRecord) => void
  requestCount: number
}

function RequestForm({
  mode,
  onCancel,
  onSubmit,
  request,
  requestCount,
}: RequestFormProps) {
  const isEdit = mode === 'edit'
  const [name, setName] = useState(request.name)
  const [type, setType] = useState(request.type)
  const [description, setDescription] = useState(request.description)
  const [items, setItems] = useState(request.items)
  const total = items.reduce(
    (sum, item) => sum + item.quantity * item.unitPrice,
    0,
  )
  const nextStatus: Status = isEdit
    ? request.status === 'Rejected'
      ? 'Resubmitted'
      : request.status
    : 'New'

  function handleSubmit() {
    const submittedAt = formatSubmittedDate(new Date())

    onSubmit({
      ...request,
      id: isEdit ? request.id : createRequestId(requestCount),
      name,
      type,
      description,
      items,
      total,
      status: nextStatus,
      reason: request.reason,
      finalRejected:
        nextStatus === 'Resubmitted' ? undefined : request.finalRejected,
      submitted: isEdit ? request.submitted : submittedAt,
      updated: 'Just now',
    })
  }

  return (
    <>
      <Topbar
        title={isEdit ? 'Edit Purchase Request' : 'New Purchase Request'}
      />
      <section className="content-area form-layout">
        <div className="panel form-panel">
          <div className="panel-heading">
            <div>
              <p className="eyebrow">{isEdit ? request.id : 'Draft request'}</p>
              <h2>
                {isEdit ? 'Edit Purchase Request' : 'Create Purchase Request'}
              </h2>
              <span>Fill in the details below. Required fields marked *</span>
            </div>
            {isEdit && (
              <StatusBadge
                finalRejected={request.finalRejected}
                status={request.status}
              />
            )}
          </div>

          {isEdit && request.status === 'Rejected' && (
            <div className="notice danger">
              <AlertTriangle size={18} />
              <div>
                <strong>Request was rejected</strong>
                <span>{request.reason}</span>
              </div>
            </div>
          )}

          <div className="form-grid">
            <Field label="Request Name *">
              <input
                onChange={(event) => setName(event.target.value)}
                value={name}
              />
            </Field>
            <Field label="Request Type *">
              <select
                onChange={(event) => setType(event.target.value)}
                value={type}
              >
                <option>Hardware</option>
                <option>Software</option>
                <option>Cloud</option>
                <option>Furniture</option>
              </select>
            </Field>
          </div>

          <div className="section-title">
            <span>
              <Layers3 size={16} />
              Requested items
            </span>
            <button
              className="btn compact"
              onClick={() =>
                setItems((currentItems) => [
                  ...currentItems,
                  {
                    name: 'New product',
                    category: 'Category',
                    quantity: 1,
                    unitPrice: 0,
                  },
                ])
              }
              type="button"
            >
              <Plus size={14} />
              Add product
            </button>
          </div>
          <div className="items-list">
            <div className="item-row item-row-head">
              <span>Product</span>
              <span>Category</span>
              <span>Qty</span>
              <span>Total</span>
            </div>
            {items.map((item, index) => (
              <div className="item-row" key={`${item.name}-${index}`}>
                <input
                  aria-label="Product name"
                  onChange={(event) =>
                    setItems((currentItems) =>
                      currentItems.map((currentItem, currentIndex) =>
                        currentIndex === index
                          ? { ...currentItem, name: event.target.value }
                          : currentItem,
                      ),
                    )
                  }
                  value={item.name}
                />
                <input
                  aria-label="Product category"
                  onChange={(event) =>
                    setItems((currentItems) =>
                      currentItems.map((currentItem, currentIndex) =>
                        currentIndex === index
                          ? { ...currentItem, category: event.target.value }
                          : currentItem,
                      ),
                    )
                  }
                  value={item.category}
                />
                <input
                  aria-label="Quantity"
                  onChange={(event) =>
                    setItems((currentItems) =>
                      currentItems.map((currentItem, currentIndex) =>
                        currentIndex === index
                          ? {
                              ...currentItem,
                              quantity: Number(event.target.value),
                            }
                          : currentItem,
                      ),
                    )
                  }
                  type="number"
                  value={item.quantity}
                />
                <strong>
                  {formatMoney(item.quantity * item.unitPrice)}
                </strong>
              </div>
            ))}
          </div>

          <Field label="Additional Details">
            <textarea
              onChange={(event) => setDescription(event.target.value)}
              rows={4}
              value={description}
            />
          </Field>
        </div>

        <aside className="panel summary-panel">
          <p className="eyebrow">Summary</p>
          <h2>{formatMoney(total)}</h2>
          <div className="summary-line">
            <span>Approver</span>
            <strong>{request.approver}</strong>
          </div>
          <div className="summary-line">
            <span>Status after submit</span>
            <strong>{nextStatus}</strong>
          </div>
          <div className="summary-line">
            <span>Currency</span>
            <strong>EUR</strong>
          </div>
          <div className="form-actions">
            <button className="btn" onClick={onCancel} type="button">
              Cancel
            </button>
            <button className="btn primary" onClick={handleSubmit} type="button">
              {isEdit ? 'Save changes' : 'Submit request'}
              <Send size={14} />
            </button>
          </div>
        </aside>
      </section>
    </>
  )
}

type FieldProps = {
  children: React.ReactNode
  label: string
}

function Field({ children, label }: FieldProps) {
  return (
    <label className="field">
      <span>{label}</span>
      {children}
    </label>
  )
}

type RequestDetailProps = {
  request: RequestRecord
  onApprove: () => void
  onBack: () => void
  onEdit: () => void
}

function RequestDetail({
  onApprove,
  onBack,
  onEdit,
  request,
}: RequestDetailProps) {
  return (
    <>
      <Topbar title="Request Details" />
      <section className="content-area detail-layout">
        <div className="panel detail-panel">
          <button className="back-button" onClick={onBack} type="button">
            ← All Requests
          </button>
          <div className="detail-title">
            <div>
              <p className="eyebrow">
                {request.id} · {request.submitted}
              </p>
              <h2>{request.name}</h2>
              <p>{request.description}</p>
            </div>
            <StatusBadge
              finalRejected={request.finalRejected}
              status={request.status}
            />
          </div>

          {request.reason && (
            <div className="notice danger">
              <AlertTriangle size={18} />
              <div>
                <strong>
                  {request.status === 'Approved'
                    ? `Previous rejection reason from ${request.approver}`
                    : request.finalRejected
                      ? `Final rejection by ${request.approver}`
                      : `Rejected by ${request.approver}`}
                </strong>
                <span>{request.reason}</span>
              </div>
            </div>
          )}

          <div className="meta-grid meta-grid-4">
            <Metric label="Requester" value={request.creator} />
            <Metric label="Request Type" value={request.type} />
            <Metric label="Approver" value={request.approver} />
            <Metric label="Total Amount" value={formatMoney(request.total)} />
          </div>

          <div className="section-title">
            <FileText size={16} />
            Request Details
          </div>
          <div className="line-items">
            {request.items.map((item) => (
              <div className="line-item" key={item.name}>
                <div>
                  <strong>{item.name}</strong>
                  <span>
                    {item.category} · Qty {item.quantity}
                  </span>
                </div>
                <strong>{formatMoney(item.quantity * item.unitPrice)}</strong>
              </div>
            ))}
          </div>
        </div>

        <aside className="panel timeline-panel">
          <p className="eyebrow">Workflow</p>
          <h2>Approval timeline</h2>
          <div className="timeline">
            <TimelineItem active label="Submitted" text={request.submitted} />
            <TimelineItem
              active={request.status !== 'New'}
              label="Manager review"
              text={`Assigned to ${request.approver}`}
            />
            <TimelineItem
              active={
                request.status === 'Approved' || request.status === 'Rejected'
              }
              label="Decision"
              text={
                request.status === 'Approved'
                  ? 'Approved'
                  : request.status === 'Rejected'
                    ? 'Rejected'
                    : 'Waiting for decision'
              }
            />
          </div>
          <div className="form-actions">
            {request.status === 'Rejected' && !request.finalRejected ? (
              <button className="btn primary" onClick={onEdit} type="button">
                Edit request
              </button>
            ) : request.status === 'Rejected' && request.finalRejected ? (
              <button className="btn" disabled type="button">
                Final decision recorded
              </button>
            ) : (
              <>
                <button className="btn" onClick={onEdit} type="button">
                  Edit
                </button>
                <button className="btn primary" onClick={onApprove} type="button">
                  Review
                </button>
              </>
            )}
          </div>
        </aside>
      </section>
    </>
  )
}

type ApprovalViewProps = {
  decision: 'idle' | 'approved' | 'rejected'
  onBack: () => void
  onDecide: (
    decision: 'approved' | 'rejected',
    reason?: string,
    finalRejected?: boolean,
  ) => void
  request: RequestRecord
}

function ApprovalView({
  decision,
  onBack,
  onDecide,
  request,
}: ApprovalViewProps) {
  const [showRejectDialog, setShowRejectDialog] = useState(false)
  const [rejectReason, setRejectReason] = useState('')
  const isDecided =
    request.status === 'Approved' || request.status === 'Rejected'

  return (
    <>
      <Topbar title="Approval Queue" />
      <section className="content-area approval-layout">
        <div className="panel detail-panel">
          <button className="back-button" onClick={onBack} type="button">
            ← Approval queue
          </button>
          <div className="detail-title">
            <div>
              <p className="eyebrow">
                Approver Role · {request.id}
              </p>
              <h2>{request.name}</h2>
              <p>{request.description}</p>
            </div>
            <StatusBadge
              finalRejected={request.finalRejected}
              status={request.status}
            />
          </div>

          {decision !== 'idle' && (
            <div className={decision === 'approved' ? 'notice success' : 'notice danger'}>
              {decision === 'approved' ? <Check size={18} /> : <X size={18} />}
              <div>
                <strong>
                  Request {decision === 'approved' ? 'approved' : 'rejected'}
                </strong>
                <span>
                  The status is now reflected in All Requests.
                </span>
              </div>
            </div>
          )}

          <div className="meta-grid meta-grid-3">
            <Metric label="Requester" value={request.creator} />
            <Metric label="Total Amount" value={formatMoney(request.total)} />
            <Metric label="Submitted" value={request.submitted} />
          </div>

          <div className="line-items">
            {request.items.map((item) => (
              <div className="line-item" key={item.name}>
                <div>
                  <strong>{item.name}</strong>
                  <span>
                    {item.category} · Qty {item.quantity}
                  </span>
                </div>
                <strong>{formatMoney(item.quantity * item.unitPrice)}</strong>
              </div>
            ))}
          </div>
        </div>

        <aside className="panel approval-panel">
          <div className="approval-icon">
            <ShieldCheck size={22} />
          </div>
          <p className="eyebrow">Approver Actions</p>
          <h2>Review decision</h2>
          <p className="approval-copy">
            {isDecided
              ? 'Decision recorded. You can return to the request list to see the updated status.'
              : 'Approve immediately, reject for resubmission, or final reject the request.'}
          </p>
          {!isDecided && (
            <div className="form-actions stacked">
              <button
                className="btn success"
                onClick={() => onDecide('approved')}
                type="button"
              >
                <Check size={15} />
                Approve request
              </button>
              <button
                className="btn danger"
                onClick={() => setShowRejectDialog(true)}
                type="button"
              >
                <X size={15} />
                Reject request
              </button>
            </div>
          )}
        </aside>

        {showRejectDialog && (
          <div
            aria-labelledby="reject-title"
            aria-modal="true"
            className="modal-backdrop"
            role="dialog"
          >
            <div className="modal">
              <div className="modal-icon danger">
                <AlertTriangle size={20} />
              </div>
              <h2 id="reject-title">Reject request</h2>
              <p>
                Use reject when the requester may edit and resubmit. Use final
                reject when the decision should be closed.
              </p>
              <Field label="Rejection reason">
                <textarea
                  autoFocus
                  onChange={(event) => setRejectReason(event.target.value)}
                  placeholder="Provide a reason if rejecting..."
                  rows={5}
                  value={rejectReason}
                />
              </Field>
              <div className="modal-actions">
                <button
                  className="btn"
                  onClick={() => setShowRejectDialog(false)}
                  type="button"
                >
                  Cancel
                </button>
                <button
                  className="btn danger"
                  onClick={() => {
                    onDecide('rejected', rejectReason, false)
                    setShowRejectDialog(false)
                  }}
                  type="button"
                >
                  Reject
                </button>
                <button
                  className="btn danger solid"
                  onClick={() => {
                    onDecide('rejected', rejectReason, true)
                    setShowRejectDialog(false)
                  }}
                  type="button"
                >
                  Final reject
                </button>
              </div>
            </div>
          </div>
        )}
      </section>
    </>
  )
}

type MetricProps = {
  label: string
  value: string
}

function Metric({ label, value }: MetricProps) {
  return (
    <div className="metric">
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  )
}

type TimelineItemProps = {
  active: boolean
  label: string
  text: string
}

function TimelineItem({ active, label, text }: TimelineItemProps) {
  return (
    <div className={active ? 'timeline-item active' : 'timeline-item'}>
      <span />
      <div>
        <strong>{label}</strong>
        <p>{text}</p>
      </div>
    </div>
  )
}

function StatusBadge({
  finalRejected = false,
  status,
}: {
  finalRejected?: boolean
  status: Status
}) {
  const label = finalRejected && status === 'Rejected' ? 'Final rejected' : status

  return <span className={`status status-${status.toLowerCase()}`}>{label}</span>
}

export default App
