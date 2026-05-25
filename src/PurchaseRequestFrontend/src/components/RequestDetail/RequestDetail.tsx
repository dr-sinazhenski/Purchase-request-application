import { AlertTriangle, FileText, Trash2 } from 'lucide-react'
import { useState } from 'react'

import type { RequestRecord } from '../../types'
import { formatMoney } from '../../utils/format'
import { Metric } from '../Metric/Metric'
import { StatusBadge } from '../StatusBadge/StatusBadge'
import { TimelineItem } from '../TimelineItem/TimelineItem'
import { Topbar } from '../Topbar/Topbar'
import './RequestDetail.css'

type RequestDetailProps = {
  request: RequestRecord
  onApprove: () => void
  onBack: () => void
  onDelete: (id: string) => Promise<void>
  onEdit: () => void
  canDelete: boolean
  canEdit: boolean
  canReview: boolean
}

export function RequestDetail({
  canDelete,
  canEdit,
  canReview,
  onApprove,
  onBack,
  onDelete,
  onEdit,
  request,
}: RequestDetailProps) {
  const [deleteError, setDeleteError] = useState('')
  const [isDeleting, setIsDeleting] = useState(false)

  async function handleDelete() {
    const confirmed = window.confirm(
      `Delete request "${request.name}"? This cannot be undone.`,
    )

    if (!confirmed) {
      return
    }

    try {
      setDeleteError('')
      setIsDeleting(true)
      await onDelete(request.id)
    } catch (error) {
      setDeleteError(
        error instanceof Error
          ? error.message
          : 'Failed to delete request. Please try again.',
      )
      setIsDeleting(false)
    }
  }

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
              <div className="description-block">
                <strong>Description</strong>
                <p>{request.description}</p>
              </div>
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

          {deleteError && (
            <div className="notice danger">
              <AlertTriangle size={18} />
              <div>
                <strong>Delete failed</strong>
                <span>{deleteError}</span>
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
            {canEdit && request.status === 'Rejected' && !request.finalRejected ? (
              <button className="btn primary" onClick={onEdit} type="button">
                Edit request
              </button>
            ) : request.status === 'Rejected' && request.finalRejected ? (
              <button className="btn" disabled type="button">
                Final decision recorded
              </button>
            ) : canEdit || canReview ? (
              <>
                {canEdit && (
                  <button className="btn" onClick={onEdit} type="button">
                    Edit
                  </button>
                )}
                {canReview && (
                  <button className="btn primary" onClick={onApprove} type="button">
                    Review
                  </button>
                )}
              </>
            ) : (
              <button className="btn" disabled type="button">
                View only
              </button>
            )}
            {canDelete && (
              <button
                className="btn danger"
                disabled={isDeleting}
                onClick={handleDelete}
                type="button"
              >
                <Trash2 size={14} />
                {isDeleting ? 'Deleting...' : 'Delete'}
              </button>
            )}
          </div>
        </aside>
      </section>
    </>
  )
}
