import { AlertTriangle, Check, ShieldCheck, X } from 'lucide-react'
import { useState } from 'react'

import type { DecisionState, RequestRecord } from '../../types'
import { formatMoney } from '../../utils/format'
import { Field } from '../Field/Field'
import { Metric } from '../Metric/Metric'
import { StatusBadge } from '../StatusBadge/StatusBadge'
import { Topbar } from '../Topbar/Topbar'
import './ApprovalView.css'

type ApprovalViewProps = {
  decision: DecisionState
  onBack: () => void
  onDecide: (
    decision: 'approved' | 'rejected',
    reason?: string,
    finalRejected?: boolean,
  ) => void
  request: RequestRecord
}

export function ApprovalView({
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
            <div
              className={decision === 'approved' ? 'notice success' : 'notice danger'}
            >
              {decision === 'approved' ? <Check size={18} /> : <X size={18} />}
              <div>
                <strong>
                  Request {decision === 'approved' ? 'approved' : 'rejected'}
                </strong>
                <span>The status is now reflected in All Requests.</span>
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
