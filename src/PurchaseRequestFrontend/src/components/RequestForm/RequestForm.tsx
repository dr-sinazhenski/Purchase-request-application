import { AlertTriangle, Layers3, Plus, Send, Trash2 } from 'lucide-react'
import { useEffect, useState } from 'react'

import type { RequestRecord, Status } from '../../types'
import {
  formatMoney,
  formatSubmittedDate,
} from '../../utils/format'
import {
  createRequestApi,
  loadRequestTypes,
  loadProducts,
  loadPrices,
  loadRegions,
  updateRequestApi,
} from '../../api'
import type {
  AccountOption,
  PriceOption,
  ProductOption,
  RegionOption,
  RequestTypeOption,
} from '../../api'
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

function mapItemsToProductAmounts(items: RequestRecord['items']) {
  return items.reduce<Record<string, number>>((amounts, item) => {
    if (item.productId && item.quantity > 0) {
      amounts[item.productId] = (amounts[item.productId] ?? 0) + item.quantity
    }

    return amounts
  }, {})
}

function parseApiDate(value: string | undefined) {
  return value ? new Date(value) : new Date()
}

function getCurrencyFromToken(token?: string) {
  if (!token) {
    return undefined
  }

  try {
    const payload = token.split('.')[1]
    const normalizedPayload = payload.replace(/-/g, '+').replace(/_/g, '/')
    const parsedPayload = JSON.parse(atob(normalizedPayload)) as {
      currency?: string
    }

    return parsedPayload.currency
  } catch {
    return undefined
  }
}

function createBlankItem(): RequestRecord['items'][number] {
  return {
    name: '',
    category: '',
    quantity: 1,
    unitPrice: 0,
    productId: undefined,
  }
}

function validateRequestForm(
  name: string,
  requestTypeId: string,
  items: RequestRecord['items'],
  products: ProductOption[],
) {
  if (!name.trim()) {
    return 'Please enter a request name.'
  }

  if (!requestTypeId) {
    return 'Please select a request type before submitting.'
  }

  const selectedItems = items.filter((item) => item.productId)

  if (selectedItems.length === 0) {
    return 'Please add at least one product to the request.'
  }

  const invalidProduct = selectedItems.find((item) => {
    const product = products.find((option) => option.id === item.productId)

    return !product || !product.requestTypeIds.includes(requestTypeId)
  })

  if (invalidProduct) {
    return 'One or more selected products are not available for this request type.'
  }

  const invalidQuantity = selectedItems.find(
    (item) => !Number.isFinite(item.quantity) || item.quantity < 1,
  )

  if (invalidQuantity) {
    return 'Product quantity must be at least 1.'
  }

  const invalidPrice = selectedItems.find(
    (item) => !Number.isFinite(item.unitPrice) || item.unitPrice < 0,
  )

  if (invalidPrice) {
    return 'Product price cannot be negative.'
  }

  return ''
}

type RequestFormProps = {
  currentAccount?: AccountOption
  mode: 'create' | 'edit'
  request: RequestRecord
  onCancel: () => void
  onSubmit: (request: RequestRecord) => void | Promise<void>
}

export function RequestForm({
  currentAccount,
  mode,
  onCancel,
  onSubmit,
  request,
}: RequestFormProps) {
  const isEdit = mode === 'edit'
  const [name, setName] = useState(request.name)
  const [description, setDescription] = useState(request.description)
  const [items, setItems] = useState(request.items)
  const [requestTypes, setRequestTypes] = useState<RequestTypeOption[]>([])
  const [selectedRequestTypeId, setSelectedRequestTypeId] = useState('')
  const [isLoadingRequestTypes, setIsLoadingRequestTypes] = useState(false)
  const [requestTypeError, setRequestTypeError] = useState('')
  const [products, setProducts] = useState<ProductOption[]>([])
  const [isLoadingProducts, setIsLoadingProducts] = useState(false)
  const [productError, setProductError] = useState('')
  const [prices, setPrices] = useState<PriceOption[]>([])
  const [regions, setRegions] = useState<RegionOption[]>([])
  const [selectedRegionId, setSelectedRegionId] = useState('')
  const [priceError, setPriceError] = useState('')
  const [submitError, setSubmitError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  const accountRegionId = currentAccount?.regionId ?? ''
  const accountCurrency = getCurrencyFromToken(currentAccount?.token)
  const selectedRegion = regions.find((region) => region.id === selectedRegionId)
  const currency = accountCurrency ?? selectedRegion?.currency ?? 'EUR'
  const total = items.reduce(
    (sum, item) => sum + item.quantity * item.unitPrice,
    0,
  )
  const nextStatus: Status = isEdit
    ? request.status === 'For Revision'
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
        }
      } catch {
        setRequestTypeError('Unable to load request types from API.')
      } finally {
        if (mounted) {
          setIsLoadingRequestTypes(false)
        }
      }
    }

    async function loadProds() {
      setIsLoadingProducts(true)
      setProductError('')

      try {
        const result = await loadProducts()
        if (!mounted) return

        if (!result.isSuccess || !result.data) {
          setProductError('Unable to load products from API.')
          return
        }

        setProducts(result.data)
      } catch {
        setProductError('Unable to load products from API.')
      } finally {
        if (mounted) {
          setIsLoadingProducts(false)
        }
      }
    }

    async function loadPriceData() {
      setPriceError('')

      try {
        const [priceResult, regionResult] = await Promise.all([
          loadPrices(),
          loadRegions(),
        ])
        if (!mounted) return

        if (!priceResult.isSuccess || !priceResult.data) {
          setPriceError('Unable to load prices from API.')
          return
        }

        if (!regionResult.isSuccess || !regionResult.data) {
          setPriceError('Unable to load regions from API.')
          return
        }

        setPrices(priceResult.data)
        setRegions(regionResult.data)

        const accountRegion = regionResult.data.find(
          (region) => region.id === accountRegionId,
        )
        const accountCurrencyRegion = regionResult.data.find(
          (region) => region.currency === accountCurrency,
        )
        const europeRegion = regionResult.data.find(
          (region) => region.name === 'Europe',
        )
        const initialRegion =
          accountRegion ??
          accountCurrencyRegion ??
          europeRegion ??
          regionResult.data[0]

        if (initialRegion) {
          setSelectedRegionId(initialRegion.id)
        }
      } catch {
        setPriceError('Unable to load prices from API.')
      }
    }

    loadTypes()
    loadProds()
    loadPriceData()

    return () => {
      mounted = false
    }
  }, [accountCurrency, accountRegionId, request.type])

  function findPrice(productId: string, regionId = selectedRegionId) {
    return prices.find(
      (price) => price.productId === productId && price.regionId === regionId,
    )
  }

  useEffect(() => {
    if (!selectedRegionId || prices.length === 0) {
      return
    }

    setItems((currentItems) =>
      currentItems.map((currentItem) => {
        if (!currentItem.productId) {
          return currentItem
        }

        const price = findPrice(currentItem.productId)

        if (!price) {
          return currentItem
        }

        return {
          ...currentItem,
          category: price.unitsOfMeasure,
          unitPrice: price.amount,
        }
      }),
    )
  }, [prices, selectedRegionId])

  async function handleSubmit() {
    const validationError = validateRequestForm(
      name,
      selectedRequestTypeId,
      items,
      products,
    )

    if (validationError) {
      setSubmitError(validationError)
      return
    }

    if (!isEdit) {
      try {
        setSubmitError('')
        setIsSubmitting(true)
        const result = await createRequestApi({
          title: name.trim(),
          description: description.trim(),
          requestTypeId: selectedRequestTypeId,
          productIdAmount: mapItemsToProductAmounts(items),
        })

        if (!result.isSuccess || !result.data) {
          setSubmitError('Failed to create request.')
          setIsSubmitting(false)
          return
        }

        const formItems = items.filter((item) => item.productId)
        const formTotal = formItems.reduce(
          (sum, item) => sum + item.quantity * item.unitPrice,
          0,
        )
        const creatorName = currentAccount?.name ?? 'Current user'

        const createdRequest: RequestRecord = {
          id: result.data.id,
          name: result.data.title,
          type: result.data.requestType.name,
          typeId: result.data.requestType.id,
          status: mapBackendStatus(result.data.status),
          total: formTotal,
          creator: creatorName,
          initials:
            creatorName
              .split(' ')
              .filter(Boolean)
              .map((part) => part[0])
              .join('')
              .slice(0, 2)
              .toUpperCase() || 'CU',
          updated: 'Just now',
          submitted: formatSubmittedDate(parseApiDate(result.data.createdAt)),
          approver: currentAccount?.approverProfileName ?? 'Approval queue',
          description: result.data.description ?? description,
          items: formItems,
          ownerAccountId: currentAccount?.id,
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
          title: name.trim(),
          description: description.trim(),
          requestTypeId: selectedRequestTypeId,
          productIdAmount: mapItemsToProductAmounts(items),
        })

        if (!result.isSuccess || !result.data) {
          setSubmitError('Failed to update request.')
          setIsSubmitting(false)
          return
        }

        const formItems = items.filter((item) => item.productId)
        const formTotal = formItems.reduce(
          (sum, item) => sum + item.quantity * item.unitPrice,
          0,
        )
        const updatedStatus = mapBackendStatus(result.data.status)

        const updatedRequest: RequestRecord = {
          id: result.data.id,
          name: result.data.title,
          type: result.data.requestType.name,
          typeId: result.data.requestType.id,
          status: updatedStatus,
          total: formTotal,
          creator: request.creator,
          initials: request.initials,
          updated: 'Just now',
          submitted: request.submitted,
          approver: request.approver,
          description: result.data.description ?? description,
          items: formItems,
          reason: updatedStatus === 'For Revision' ? request.reason : undefined,
          finalRejected:
            updatedStatus === 'Rejected' ? request.finalRejected : undefined,
          ownerAccountId: request.ownerAccountId,
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
              <div className="request-form-status">
                <StatusBadge
                  finalRejected={request.finalRejected}
                  status={request.status}
                />
                {request.reason && (
                  <span
                    className={
                      request.status === 'Rejected'
                        ? 'request-form-status-reason final'
                        : 'request-form-status-reason returned'
                    }
                  >
                    {request.reason}
                  </span>
                )}
              </div>
            )}
          </div>

          {(requestTypeError || productError || priceError || submitError) && (
            <div className="notice danger">
              <AlertTriangle size={18} />
              <div>
                <strong>Error</strong>
                <span>
                  {requestTypeError || productError || priceError || submitError}
                </span>
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
                  setSelectedRequestTypeId(selectedId)
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
                  createBlankItem(),
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
              <span>Unit</span>
              <span>Qty</span>
              <span>Price</span>
              <span>Total</span>
              <span />
            </div>
            {items.map((item, index) => {
              const availableProducts = products.filter((product) =>
                product.requestTypeIds.includes(selectedRequestTypeId),
              )
              return (
                <div className="item-row" key={index}>
                  <select
                    aria-label="Product"
                    disabled={isLoadingProducts}
                    onChange={(event) => {
                      const selectedProductId = event.target.value
                      const selectedProduct = products.find(
                        (p) => p.id === selectedProductId,
                      )
                      const selectedPrice = selectedProductId
                        ? findPrice(selectedProductId)
                        : undefined
                      setItems((currentItems) =>
                        currentItems.map((currentItem, currentIndex) =>
                          currentIndex === index
                            ? {
                                ...currentItem,
                                productId: selectedProductId || undefined,
                                name: selectedProduct?.name ?? '',
                                category:
                                  selectedPrice?.unitsOfMeasure ??
                                  currentItem.category,
                                unitPrice:
                                  selectedPrice?.amount ?? currentItem.unitPrice,
                              }
                            : currentItem,
                        ),
                      )
                    }}
                    value={item.productId ?? ''}
                  >
                    {isLoadingProducts && (
                      <option value="">Loading products...</option>
                    )}
                    {!isLoadingProducts && availableProducts.length === 0 && (
                      <option value="">No products available for this type</option>
                    )}
                    <option value="">Select a product...</option>
                    {availableProducts.map((product) => (
                      <option key={product.id} value={product.id}>
                        {product.name}
                      </option>
                    ))}
                  </select>
                <input
                  aria-label="Unit of measure"
                  placeholder="Unit"
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
                <strong>
                  {formatMoney(item.quantity * item.unitPrice, currency)}
                </strong>
                <button
                  aria-label={`Remove product ${index + 1}`}
                  className="remove-item-button"
                  onClick={() =>
                    setItems((currentItems) =>
                      currentItems.length === 1
                        ? [createBlankItem()]
                        : currentItems.filter(
                            (_currentItem, currentIndex) =>
                              currentIndex !== index,
                          ),
                    )
                  }
                  title="Remove product"
                  type="button"
                >
                  <Trash2 size={15} />
                  Delete
                </button>
              </div>
            ) })}
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
          <h2>{formatMoney(total, currency)}</h2>
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
            <strong>{currency}</strong>
          </div>
        </aside>
        <div className="form-actions request-form-actions">
          <button className="btn" onClick={onCancel} type="button">
            Cancel
          </button>
          <button
            className="btn primary"
            disabled={isSubmitting || isLoadingRequestTypes}
            onClick={handleSubmit}
            type="button"
          >
            {isEdit ? 'Save changes' : isSubmitting ? 'Submitting...' : 'Submit request'}
            <Send size={14} />
          </button>
        </div>
      </section>
    </>
  )
}
