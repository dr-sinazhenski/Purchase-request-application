import {
  AlertTriangle,
  Check,
  ShieldCheck,
  Undo2,
  X,
} from 'lucide-react'
import { useState } from 'react'

import type { DecisionState, RequestRecord, RequestSort } from '../../types'
import { formatMoney, getSortableDate } from '../../utils/format'
import { Field } from '../Field/Field'
import { Metric } from '../Metric/Metric'
import { SortMenu } from '../SortMenu/SortMenu'
import { StatusBadge } from '../StatusBadge/StatusBadge'
import { Topbar } from '../Topbar/Topbar'
import './ApprovalView.css'

type ApprovalViewProps = {
  currency: string
  decision: DecisionState
  onBack: () => void
  onDecide: (
    decision: 'approved' | 'rejected',
    reason?: string,
    finalRejected?: boolean,
  ) => void | Promise<void>
  onOpenRequest: (request: RequestRecord) => void
  request?: RequestRecord
  requests: RequestRecord[]
}

export function ApprovalView({
  currency,
  decision,
  onBack,
  onDecide,
  onOpenRequest,
  request,
  requests,
}: ApprovalViewProps) {
  const [showReturnDialog, setShowReturnDialog] = useState(false)
  const [showRejectDialog, setShowRejectDialog] = useState(false)
  const [rejectReason, setRejectReason] = useState('')
  const [decisionError, setDecisionError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [queueSearch, setQueueSearch] = useState('')
  const [queueSort, setQueueSort] = useState<RequestSort>('newest')
  const isDecided =
    request?.status === 'Approved' ||
    request?.status === 'For Revision' ||
    request?.status === 'Rejected'
  const normalizedQueueSearch = queueSearch.trim().toLowerCase()
  const visibleRequests = requests
    .filter((queuedRequest) => {
      if (!normalizedQueueSearch) {
        return true
      }

      return [
        queuedRequest.id,
        queuedRequest.name,
        queuedRequest.type,
        queuedRequest.status,
        queuedRequest.creator,
        queuedRequest.description,
        queuedRequest.total.toString(),
        ...queuedRequest.items.flatMap((item) => [
          item.name,
          item.category,
          item.quantity.toString(),
          item.unitPrice.toString(),
        ]),
      ]
        .join(' ')
        .toLowerCase()
        .includes(normalizedQueueSearch)
    })
    .sort((first, second) => {
      switch (queueSort) {
        case 'oldest':
          return getSortableDate(first.submitted) - getSortableDate(second.submitted)
        case 'priceHigh':
          return second.total - first.total
        case 'priceLow':
          return first.total - second.total
        default:
          return getSortableDate(second.submitted) - getSortableDate(first.submitted)
      }
    })

  async function submitDecision(
    nextDecision: 'approved' | 'rejected',
    reason = '',
    finalRejected = false,
  ) {
    try {
      setDecisionError('')
      setIsSubmitting(true)
      await onDecide(nextDecision, reason, finalRejected)
      setShowReturnDialog(false)
      setShowRejectDialog(false)
      setRejectReason('')
    } catch (error) {
      setDecisionError(
        error instanceof Error
          ? error.message
          : 'Failed to record decision. Please try again.',
      )
    } finally {
      setIsSubmitting(false)
    }
  }

  function closeDecisionDialog() {
    setShowReturnDialog(false)
    setShowRejectDialog(false)
    setRejectReason('')
  }

  return (
    <>
      <Topbar
        searchPlaceholder="Search approval queue"
        searchValue={queueSearch}
        title="Approval Queue"
        onSearch={!request ? setQueueSearch : undefined}
      />
      {!request ? (
        <section className="content-area approval-list-layout">
          <div className="panel queue-panel queue-panel-wide">
            <div className="queue-header">
              <p className="eyebrow">Waiting for review</p>
              <strong>{visibleRequests.length}</strong>
            </div>

            <div className="queue-controls">
              <span>
                Showing {visibleRequests.length} of {requests.length}
              </span>
              <SortMenu onChange={setQueueSort} value={queueSort} />
            </div>

            {visibleRequests.length === 0 ? (
              <p className="queue-empty">
                {requests.length === 0
                  ? 'No requests are waiting for approval.'
                  : 'No requests match your search.'}
              </p>
            ) : (
              <div className="queue-list">
                {visibleRequests.map((queuedRequest) => (
                  <button
                    className="queue-item"
                    key={queuedRequest.id}
                    onClick={() => onOpenRequest(queuedRequest)}
                    type="button"
                  >
                    <span>
                      <strong>{queuedRequest.name}</strong>
                      <small>
                        {queuedRequest.type} -{' '}
                        {formatMoney(queuedRequest.total, currency)}
                      </small>
                    </span>
                    <StatusBadge
                      finalRejected={queuedRequest.finalRejected}
                      status={queuedRequest.status}
                    />
                  </button>
                ))}
              </div>
            )}
          </div>
        </section>
      ) : (
        <section
          className={
            showRejectDialog
              || showReturnDialog
              ? 'content-area approval-layout approval-layout-modal-open'
              : 'content-area approval-layout'
          }
        >
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
            <div
              className={
                decision === 'approved'
                  ? 'notice success'
                  : decision === 'returned'
                    ? 'notice revision'
                    : 'notice danger'
              }
            >
              {decision === 'approved' ? (
                <Check size={18} />
              ) : decision === 'returned' ? (
                <Undo2 size={18} />
              ) : (
                <X size={18} />
              )}
              <div>
                <strong>
                  {decision === 'approved'
                    ? 'Request approved'
                    : decision === 'returned'
                      ? 'Request returned for revision'
                      : 'Request rejected'}
                </strong>
              </div>
            </div>
          )}

          {decisionError && (
            <div className="notice danger">
              <AlertTriangle size={18} />
              <div>
                <strong>Decision failed</strong>
                <span>{decisionError}</span>
              </div>
            </div>
          )}

          <div className="meta-grid meta-grid-3">
            <Metric label="Requester" value={request.creator} />
            <Metric
              label="Total Amount"
              value={formatMoney(request.total, currency)}
            />
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
                <strong>
                  {formatMoney(item.quantity * item.unitPrice, currency)}
                </strong>
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
              : 'Approve immediately, return for revision, or reject the request.'}
          </p>
          {!isDecided && (
            <div className="form-actions stacked">
              <button
                className="btn success"
                disabled={isSubmitting}
                onClick={() => submitDecision('approved')}
                type="button"
              >
                <Check size={15} />
                {isSubmitting ? 'Approving...' : 'Approve request'}
              </button>
              <button
                className="btn revision"
                disabled={isSubmitting}
                onClick={() => setShowReturnDialog(true)}
                type="button"
              >
                <Undo2 size={15} />
                Return for revision
              </button>
              <button
                className="btn danger"
                disabled={isSubmitting}
                onClick={() => setShowRejectDialog(true)}
                type="button"
              >
                <X size={15} />
                Reject request
              </button>
            </div>
          )}
        </aside>

        {showReturnDialog && (
          <div
            aria-labelledby="return-title"
            aria-modal="true"
            className="modal-backdrop"
            role="dialog"
          >
            <div className="modal">
              <div className="modal-icon revision">
                <Undo2 size={20} />
              </div>
              <h2 id="return-title">Return for revision</h2>
              <p>
                Enter the reason so the requester knows what to change before
                resubmitting.
              </p>
              <Field label="Return reason">
                <textarea
                  autoFocus
                  onChange={(event) => setRejectReason(event.target.value)}
                  placeholder="Provide a reason for returning the request..."
                  rows={5}
                  value={rejectReason}
                />
              </Field>
              <div className="modal-actions">
                <button
                  className="btn"
                  disabled={isSubmitting}
                  onClick={closeDecisionDialog}
                  type="button"
                >
                  Cancel
                </button>
                <button
                  className="btn revision"
                  disabled={isSubmitting}
                  onClick={() => submitDecision('rejected', rejectReason, false)}
                  type="button"
                >
                  {isSubmitting ? 'Returning...' : 'Return'}
                </button>
              </div>
            </div>
          </div>
        )}

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
                Enter the rejection reason. This decision will close the
                request.
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
                  disabled={isSubmitting}
                  onClick={closeDecisionDialog}
                  type="button"
                >
                  Cancel
                </button>
                <button
                  className="btn danger"
                  disabled={isSubmitting}
                  onClick={() => submitDecision('rejected', rejectReason, true)}
                  type="button"
                >
                  {isSubmitting ? 'Rejecting...' : 'Reject'}
                </button>
              </div>
            </div>
          </div>
        )}
      </section>
      )}
    </>
  )
}
