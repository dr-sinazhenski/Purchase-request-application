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

type Status = 'New' | 'Resubmitted' | 'Pending' | 'Approved' | 'Rejected'
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
  items: LineItem[]
}

const statusFilters: Array<'All' | Status> = [
  'All',
  'New',
  'Resubmitted',
  'Approved',
  'Rejected',
]

const requests: RequestRecord[] = [
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
    id: 'REQ-0040',
    name: 'AWS Reserved Instances - Q2',
    type: 'Cloud',
    status: 'New',
    total: 17500,
    creator: 'Marco Polo',
    initials: 'MR',
    updated: '6 days ago',
    submitted: 'Dec 12, 2024 at 11:10',
    approver: 'Sarah Chlen',
    description: 'Q2 infrastructure capacity reservation.',
    items: [
      {
        name: 'Compute reservation',
        category: 'Infrastructure',
        quantity: 1,
        unitPrice: 17500,
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

function App() {
  const [screen, setScreen] = useState<Screen>('requests')
  const [selectedId, setSelectedId] = useState('REQ-0042')
  const [filter, setFilter] = useState<'All' | Status>('All')
  const [typeFilter, setTypeFilter] = useState<string>('All')
  const [decision, setDecision] = useState<'idle' | 'approved' | 'rejected'>(
    'idle',
  )
  const [visibleCount, setVisibleCount] = useState(6)
  const uniqueTypes = ['All', ...new Set(requests.map(r => r.type))]

  const selectedRequest =
    requests.find((request) => request.id === selectedId) ?? requests[0]

const allFilteredRequests = requests.filter(request => {
  const matchesStatus = filter === 'All' || request.status === filter
  const matchesType = typeFilter === 'All' || request.type === typeFilter
  return matchesStatus && matchesType
})

const filteredRequests = allFilteredRequests.slice(0, visibleCount)

  function openRequest(request: RequestRecord, target: Screen = 'detail') {
    setSelectedId(request.id)
    setDecision('idle')
    setScreen(target)
  }

  return (
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
              onClick={() => openRequest(requests[0], 'approval')}
              type="button"
            >
              <ClipboardCheck size={16} />
              Pending Approval
              <span className="nav-count">7</span>
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
                  typeFilter={typeFilter}
                  uniqueTypes={uniqueTypes}
                  filteredRequests={filteredRequests}
                  onCreate={() => setScreen('create')}
                  onFilter={(f) => { setFilter(f); setVisibleCount(6) }}
                  onTypeFilter={(t) => { setTypeFilter(t); setVisibleCount(6) }}
                  onOpen={openRequest}
                  visibleCount={visibleCount}
                  totalFiltered={allFilteredRequests.length}
                  onShowMore={() => setVisibleCount(c => c + 6)}
                  />
          )}

          {screen === 'create' && (
            <RequestForm
              mode="create"
              request={draftRequest}
              onCancel={() => setScreen('requests')}
              onSubmit={() => {
                setSelectedId('REQ-0042')
                setScreen('detail')
              }}
            />
          )}

          {screen === 'edit' && (
            <RequestForm
              mode="edit"
              request={selectedRequest}
              onCancel={() => setScreen('detail')}
              onSubmit={() => setScreen('detail')}
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
              onDecide={setDecision}
              request={selectedRequest}
            />
          )}
        </main>
      </div>
  )
}

type RequestsListProps = {
  filter: 'All' | Status
  typeFilter: string
  uniqueTypes: string[]
  filteredRequests: RequestRecord[]
  onCreate: () => void
  onFilter: (filter: 'All' | Status) => void
  onTypeFilter: (type: string) => void
  onOpen: (request: RequestRecord, target?: Screen) => void
  visibleCount: number
  totalFiltered: number
  onShowMore: () => void
}
function RequestsList({
  filter,
  typeFilter,
  uniqueTypes,
  filteredRequests,
  onCreate,
  onFilter,
  onTypeFilter,
  onOpen,
  visibleCount,
  totalFiltered,
  onShowMore,
}: RequestsListProps) {
  return (
    <>
      <Topbar
        count="48 total"
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
<div
  style={{ display: 'flex', gap: '6px', overflowX: 'auto', maxWidth: '300px', scrollbarWidth: 'none', cursor: 'grab' }}
  onWheel={(e) => {
    e.preventDefault()
    e.currentTarget.scrollLeft += e.deltaY
  }}
  onMouseDown={(e) => {
    const el = e.currentTarget
    el.style.cursor = 'grabbing'
    const startX = e.pageX - el.offsetLeft
    const scrollLeft = el.scrollLeft

    const onMove = (ev: MouseEvent) => {
      const x = ev.pageX - el.offsetLeft
      el.scrollLeft = scrollLeft - (x - startX)
    }
    const onUp = () => {
      el.style.cursor = 'grab'
      window.removeEventListener('mousemove', onMove)
      window.removeEventListener('mouseup', onUp)
    }

    window.addEventListener('mousemove', onMove)
    window.addEventListener('mouseup', onUp)
  }}
>
  {uniqueTypes.map((type) => (
    <button
      className={typeFilter === type ? 'chip active' : 'chip'}
      key={type}
      onClick={() => onTypeFilter(type)}
      type="button"
      style={{ flexShrink: 0 }}
    >
      {type}
    </button>
  ))}
</div>

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
                    <StatusBadge status={request.status} />
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
                          request.status === 'Pending' ||
                            request.status === 'Resubmitted'
                            ? 'approval'
                            : 'detail',
                        )
                      }
                      type="button"
                    >
                      {request.status === 'Resubmitted' ? 'Review' : 'View'}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

       <div className="table-footer">
  <span>Showing {filteredRequests.length} of 48 requests</span>
  {visibleCount < totalFiltered && (
    <button className="btn compact" onClick={onShowMore} type="button">
      Show more
    </button>
  )}
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
  onSubmit: () => void
}

function RequestForm({ mode, onCancel, onSubmit, request }: RequestFormProps) {
  const isEdit = mode === 'edit'
  const [items, setItems] = useState(request.items)
  const total = items.reduce(
    (sum, item) => sum + item.quantity * item.unitPrice,
    0,
  )

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
            {isEdit && <StatusBadge status={request.status} />}
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
              <input defaultValue={request.name} />
            </Field>
            <Field label="Request Type *">
              <select defaultValue={request.type}>
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
            <textarea defaultValue={request.description} rows={4} />
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
            <strong>Pending</strong>
          </div>
          <div className="summary-line">
            <span>Currency</span>
            <strong>EUR</strong>
          </div>
          <div className="form-actions">
            <button className="btn" onClick={onCancel} type="button">
              Cancel
            </button>
            <button className="btn primary" onClick={onSubmit} type="button">
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
            <StatusBadge status={request.status} />
          </div>

          {request.reason && (
            <div className="notice danger">
              <AlertTriangle size={18} />
              <div>
                <strong>Rejected by {request.approver}</strong>
                <span>{request.reason}</span>
              </div>
            </div>
          )}

          <div className="meta-grid">
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
            {request.status === 'Rejected' ? (
              <button className="btn primary" onClick={onEdit} type="button">
                Edit request
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
  onDecide: (decision: 'idle' | 'approved' | 'rejected') => void
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

  return (
    <>
      <Topbar title="Pending Approval" />
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
            <StatusBadge status="Pending" />
          </div>

          {decision !== 'idle' && (
            <div className={decision === 'approved' ? 'notice success' : 'notice danger'}>
              {decision === 'approved' ? <Check size={18} /> : <X size={18} />}
              <div>
                <strong>
                  Request {decision === 'approved' ? 'approved' : 'rejected'}
                </strong>
                <span>
                  This state is mocked for the presentation flow.
                </span>
              </div>
            </div>
          )}

          <div className="meta-grid">
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
            Approve immediately or reject with a reason shown to the requester.
          </p>
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
                Add a reason so the requester understands what needs to change
                before resubmitting.
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
                    onDecide('rejected')
                    setShowRejectDialog(false)
                  }}
                  type="button"
                >
                  Reject request
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

function StatusBadge({ status }: { status: Status }) {
  return <span className={`status status-${status.toLowerCase()}`}>{status}</span>
}

export default App
