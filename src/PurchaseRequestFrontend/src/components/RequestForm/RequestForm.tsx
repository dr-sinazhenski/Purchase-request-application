import { AlertTriangle, Layers3, Plus, Send } from 'lucide-react'
import { useEffect, useState } from 'react'

import type { RequestRecord, Status } from '../../types'
import {
  createRequestId,
  formatMoney,
  formatSubmittedDate,
} from '../../utils/format'
import {
  createRequestApi,
  loadRequestTypes,
  updateRequestApi,
} from '../../api'
import type { RequestTypeOption } from '../../api'
import { Field } from '../Field/Field'
import { StatusBadge } from '../StatusBadge/StatusBadge'
import { Topbar } from '../Topbar/Topbar'
import './RequestForm.css'

function mapBackendStatus(status: string): Status {
  switch (status) {
    case 'Submited':
      return 'New'
    case 'Resubmited':
      return 'Resubmitted'
    case 'Approved':
      return 'Approved'
    case 'Rejected':
      return 'Rejected'
    default:
      return 'New'
  }
}

type RequestFormProps = {
  mode: 'create' | 'edit'
  request: RequestRecord
  onCancel: () => void
  onSubmit: (request: RequestRecord) => void | Promise<void>
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
  const [requestTypes, setRequestTypes] = useState<RequestTypeOption[]>([])
  const [selectedRequestTypeId, setSelectedRequestTypeId] = useState('')
  const [isLoadingRequestTypes, setIsLoadingRequestTypes] = useState(false)
  const [requestTypeError, setRequestTypeError] = useState('')
  const [submitError, setSubmitError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  const total = items.reduce(
    (sum, item) => sum + item.quantity * item.unitPrice,
    0,
  )
  const nextStatus: Status = isEdit
    ? request.status === 'Rejected'
      ? 'Resubmitted'
      : request.status
    : 'New'

  useEffect(() => {
    let mounted = true

    async function loadTypes() {
      setIsLoadingRequestTypes(true)
      setRequestTypeError('')

      try {
        const result = await loadRequestTypes()
        if (!mounted) return

        if (!result.isSuccess || !result.data) {
          setRequestTypeError('Unable to load request types from API.')
          return
        }

        setRequestTypes(result.data)
        const existingType = result.data.find(
          (option) => option.name === request.type,
        )
        const initialType = existingType ?? result.data[0]

        if (initialType) {
          setSelectedRequestTypeId(initialType.id)
          setType(initialType.name)
        }
      } catch (error) {
        setRequestTypeError('Unable to load request types from API.')
      } finally {
        if (mounted) {
          setIsLoadingRequestTypes(false)
        }
      }
    }

    loadTypes()

    return () => {
      mounted = false
    }
  }, [request.type])

  async function handleSubmit() {
    if (!selectedRequestTypeId) {
      setSubmitError('Please select a request type before submitting.')
      return
    }

    const submittedAt = formatSubmittedDate(new Date())
    const requestPayload = {
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
    }

    if (!isEdit) {
      try {
        setSubmitError('')
        setIsSubmitting(true)
        const result = await createRequestApi({
          title: name,
          description,
          requestTypeId: selectedRequestTypeId,
        })

        if (!result.isSuccess || !result.data) {
          setSubmitError('Failed to create request.')
          setIsSubmitting(false)
          return
        }

        // Map API response to RequestRecord
        const createdRequest: RequestRecord = {
          id: result.data.id,
          name: result.data.title,
          type: result.data.requestType.name,
          status: mapBackendStatus(result.data.status),
          total,
          creator: 'Current user', // TODO: get from auth
          initials: 'CU',
          updated: 'Just now',
          submitted: formatSubmittedDate(new Date(result.data.createdAt)),
          approver: 'Sarah Chen', // TODO: get from somewhere
          description: result.data.description,
          items,
        }

        await onSubmit(createdRequest)
      } catch (error) {
        setSubmitError(
          error instanceof Error
            ? error.message
            : 'Request creation failed. Please try again.',
        )
        setIsSubmitting(false)
        return
      }
    } else {
      try {
        setSubmitError('')
        setIsSubmitting(true)
        const result = await updateRequestApi({
          id: request.id,
          title: name,
          description,
          requestTypeId: selectedRequestTypeId,
        })

        if (!result.isSuccess || !result.data) {
          setSubmitError('Failed to update request.')
          setIsSubmitting(false)
          return
        }

        const updatedRequest: RequestRecord = {
          id: result.data.id,
          name: result.data.title,
          type: result.data.requestType.name,
          status: mapBackendStatus(result.data.status),
          total,
          creator: request.creator,
          initials: request.initials,
          updated: 'Just now',
          submitted: request.submitted,
          approver: request.approver,
          description: result.data.description,
          items,
          reason: request.reason,
          finalRejected: request.finalRejected,
        }

        await onSubmit(updatedRequest)
      } catch (error) {
        setSubmitError(
          error instanceof Error
            ? error.message
            : 'Request update failed. Please try again.',
        )
        setIsSubmitting(false)
        return
      } finally {
        setIsSubmitting(false)
      }
    }
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

          {(requestTypeError || submitError) && (
            <div className="notice danger">
              <AlertTriangle size={18} />
              <div>
                <strong>Error</strong>
                <span>{requestTypeError || submitError}</span>
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
                disabled={isLoadingRequestTypes}
                onChange={(event) => {
                  const selectedId = event.target.value
                  const selectedType = requestTypes.find(
                    (option) => option.id === selectedId,
                  )
                  setSelectedRequestTypeId(selectedId)
                  setType(selectedType?.name ?? '')
                }}
                value={selectedRequestTypeId}
              >
                {isLoadingRequestTypes && (
                  <option value="">Loading request types...</option>
                )}
                {!isLoadingRequestTypes && requestTypes.length === 0 && (
                  <option value="">No request types available</option>
                )}
                {requestTypes.map((option) => (
                  <option key={option.id} value={option.id}>
                    {option.name}
                  </option>
                ))}
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
              <span>Price</span>
              <span>Total</span>
            </div>
            {items.map((item, index) => (
              <div className="item-row" key={index}>
                <input
                  aria-label="Product name"
                  placeholder="Product name"
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
                  placeholder="Category"
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
                  min="1"
                  value={item.quantity}
                />
                <input
                  aria-label="Unit price"
                  placeholder="0.00"
                  onChange={(event) =>
                    setItems((currentItems) =>
                      currentItems.map((currentItem, currentIndex) =>
                        currentIndex === index
                          ? {
                              ...currentItem,
                              unitPrice: Number(event.target.value),
                            }
                          : currentItem,
                      ),
                    )
                  }
                  type="number"
                  min="0"
                  step="0.01"
                  value={item.unitPrice}
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
            <button
              className="btn primary"
              onClick={handleSubmit}
              type="button"
              disabled={isSubmitting || isLoadingRequestTypes}
            >
              {isEdit ? 'Save changes' : isSubmitting ? 'Submitting...' : 'Submit request'}
              <Send size={14} />
            </button>
          </div>
        </aside>
      </section>
    </>
  )
}
