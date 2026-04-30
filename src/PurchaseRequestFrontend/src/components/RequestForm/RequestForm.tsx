import { AlertTriangle, Layers3, Plus, Send } from 'lucide-react'
import { useState } from 'react'

import type { RequestRecord, Status } from '../../types'
import {
  createRequestId,
  formatMoney,
  formatSubmittedDate,
} from '../../utils/format'
import { Field } from '../Field/Field'
import { StatusBadge } from '../StatusBadge/StatusBadge'
import { Topbar } from '../Topbar/Topbar'
import './RequestForm.css'

type RequestFormProps = {
  mode: 'create' | 'edit'
  request: RequestRecord
  onCancel: () => void
  onSubmit: (request: RequestRecord) => void
  requestCount: number
}

export function RequestForm({
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
                <strong>{formatMoney(item.quantity * item.unitPrice)}</strong>
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
