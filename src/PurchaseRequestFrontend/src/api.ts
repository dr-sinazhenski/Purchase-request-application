export type RequestTypeOption = {
  id: string
  name: string
}

export type RequestDetailsApiDto = {
  id: string
  title: string
  description: string
  requestType: RequestTypeOption
  status: string
  rejectionCommentText?: string
  createdAt: string
  updatedAt: string
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
}

const apiBase = import.meta.env.VITE_API_BASE_URL ?? ''

async function fetchJson<T>(input: RequestInfo, init?: RequestInit): Promise<T> {
  const url = typeof input === 'string' && input.startsWith('http') ? input : `${apiBase}${input}`

  const response = await fetch(url, init)
  const payload = await response.json()

  if (!response.ok) {
    throw new Error(payload?.error?.message ?? response.statusText)
  }

  return payload as T
}

export async function loadRequestTypes(): Promise<ApiResult<RequestTypeOption[]>> {
  return fetchJson<ApiResult<RequestTypeOption[]>>('/RequestType')
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
