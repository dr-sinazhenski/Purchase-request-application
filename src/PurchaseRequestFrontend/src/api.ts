export type RequestTypeOption = {
  id: string
  name: string
}

export type ProductOption = {
  id: string
  name: string
  description: string
  requestTypeIds: string[]
}

export type PriceOption = {
  productId: string
  regionId: string
  amount: number
  unitsOfMeasure: string
}

export type RegionOption = {
  id: string
  name: string
  currency: string
}

export type RequestDetailsApiDto = {
  id: string
  title: string
  description?: string
  requestType: RequestTypeOption
  status: string
  rejectionCommentText?: string
  createdAt?: string
  updatedAt?: string
  products?: RequestItemApiDto[]
}

export type RequestItemApiDto = {
  id: string
  name: string
  description: string
  amount: number
  price: number
}

export type ApiResult<T> = {
  isSuccess: boolean
  data?: T
  error?: {
    message?: string
  }
}

export type CreateRequestApiDto = {
  title: string
  description: string
  requestTypeId: string
  productIdAmount: Record<string, number>
}

export type UpdateRequestApiDto = CreateRequestApiDto & {
  id: string
}

export type RejectRequestApiDto = {
  id: string
  reason: string
  isFinal: boolean
}

const apiBase = import.meta.env.VITE_API_BASE_URL ?? ''

async function fetchJson<T>(input: RequestInfo, init?: RequestInit): Promise<T> {
  const url = typeof input === 'string' && input.startsWith('http') ? input : `${apiBase}${input}`

  const response = await fetch(url, init)
  const text = await response.text()
  const payload = text ? JSON.parse(text) : { isSuccess: response.ok }

  if (!response.ok) {
    throw new Error(payload?.error?.message ?? response.statusText)
  }

  return payload as T
}

export async function loadRequestTypes(): Promise<ApiResult<RequestTypeOption[]>> {
  return fetchJson<ApiResult<RequestTypeOption[]>>('/RequestType')
}

export async function loadProducts(): Promise<ApiResult<ProductOption[]>> {
  return fetchJson<ApiResult<ProductOption[]>>('/Product')
}

export async function loadPrices(): Promise<ApiResult<PriceOption[]>> {
  return fetchJson<ApiResult<PriceOption[]>>('/Price')
}

export async function loadRegions(): Promise<ApiResult<RegionOption[]>> {
  return fetchJson<ApiResult<RegionOption[]>>('/Region')
}

export async function loadRequests(): Promise<ApiResult<RequestDetailsApiDto[]>> {
  return fetchJson<ApiResult<RequestDetailsApiDto[]>>('/Request')
}

export async function loadRequestDetails(
  id: string,
): Promise<ApiResult<RequestDetailsApiDto>> {
  return fetchJson<ApiResult<RequestDetailsApiDto>>(`/Request/${id}`)
}

export async function createRequestApi(
  dto: CreateRequestApiDto,
): Promise<ApiResult<RequestDetailsApiDto>> {
  return fetchJson<ApiResult<RequestDetailsApiDto>>('/Request', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(dto),
  })
}

export async function updateRequestApi(
  dto: UpdateRequestApiDto,
): Promise<ApiResult<RequestDetailsApiDto>> {
  return fetchJson<ApiResult<RequestDetailsApiDto>>('/Request', {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(dto),
  })
}

export async function deleteRequestApi(id: string): Promise<ApiResult<void>> {
  return fetchJson<ApiResult<void>>(`/Request/${id}`, {
    method: 'DELETE',
  })
}

export async function approveRequestApi(id: string): Promise<ApiResult<void>> {
  return fetchJson<ApiResult<void>>(`/Approve/${id}`, {
    method: 'PUT',
  })
}

export async function rejectRequestApi(
  dto: RejectRequestApiDto,
): Promise<ApiResult<void>> {
  return fetchJson<ApiResult<void>>('/Reject/', {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(dto),
  })
}
