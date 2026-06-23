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

export type RoleOption = {
  id: string
  name: string
}

export type ApproverProfileOption = {
  id: string
  name: string
  minAmount: number
  maxAmount: number
}

export type AccountOption = {
  id: string
  login: string
  name: string
  regionId: string
  regionName: string
  approverProfileId?: string
  approverProfileName?: string
  roleIds: string[]
  roleNames: string[]
  token?: string
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

export type FilteredRequestApiDto = {
  id: string
  title: string
  requestType: string
  status: string
  totalPrice: number
  createdAt?: string
  updatedAt?: string
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

export type CommentApiDto = {
  id: string
  requestId: string
  accountId?: string | null
  text: string
  creationTime?: string
}

export type CreateAccountApiDto = {
  login: string
  password: string
  name: string
  regionId: string
  approverProfileId?: string | null
  roleIds: string[]
}

export type UpdateAccountApiDto = CreateAccountApiDto & {
  id: string
}

export type LoginAccountApiDto = {
  login: string
  password: string
}

export type LoginAccountApiResponse = {
  token: string
  name: string
  roles: string[]
}

export type CrudRoleApiDto = {
  name: string
}

export type CrudApproverProfileApiDto = {
  name: string
  minAmount: number
  maxAmount: number
}

const apiBase = import.meta.env.VITE_API_BASE_URL ?? ''
const authTokenStorageKey = 'procureflow.authToken'
const accountStorageKey = 'procureflow.account'

export function setAuthToken(token: string) {
  window.localStorage.setItem(authTokenStorageKey, token)
}

export function clearAuthToken() {
  window.localStorage.removeItem(authTokenStorageKey)
}

function getAuthToken() {
  const storedToken = window.localStorage.getItem(authTokenStorageKey)

  if (storedToken) {
    return storedToken
  }

  const storedAccount = window.localStorage.getItem(accountStorageKey)

  if (!storedAccount) {
    return ''
  }

  try {
    return (JSON.parse(storedAccount) as AccountOption).token ?? ''
  } catch {
    return ''
  }
}

export function hasStoredAuthToken() {
  return Boolean(getAuthToken())
}

async function fetchJson<T>(input: RequestInfo, init?: RequestInit): Promise<T> {
  const url = typeof input === 'string' && input.startsWith('http') ? input : `${apiBase}${input}`
  const token = getAuthToken()
  const headers = new Headers(init?.headers)

  if (token && !headers.has('Authorization')) {
    headers.set('Authorization', `Bearer ${token}`)
  }

  const response = await fetch(url, { ...init, headers })
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

export async function loadRoles(): Promise<ApiResult<RoleOption[]>> {
  return fetchJson<ApiResult<RoleOption[]>>('/Role')
}

export async function createRoleApi(
  dto: CrudRoleApiDto,
): Promise<ApiResult<RoleOption>> {
  return fetchJson<ApiResult<RoleOption>>('/Role', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(dto),
  })
}

export async function updateRoleApi(
  id: string,
  dto: CrudRoleApiDto,
): Promise<ApiResult<RoleOption>> {
  return fetchJson<ApiResult<RoleOption>>(`/Role/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(dto),
  })
}

export async function deleteRoleApi(id: string): Promise<ApiResult<void>> {
  return fetchJson<ApiResult<void>>(`/Role/${id}`, {
    method: 'DELETE',
  })
}

export async function loadApproverProfiles(): Promise<
  ApiResult<ApproverProfileOption[]>
> {
  return fetchJson<ApiResult<ApproverProfileOption[]>>('/ApproverProfile')
}

export async function createApproverProfileApi(
  dto: CrudApproverProfileApiDto,
): Promise<ApiResult<ApproverProfileOption>> {
  return fetchJson<ApiResult<ApproverProfileOption>>('/ApproverProfile', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(dto),
  })
}

export async function updateApproverProfileApi(
  id: string,
  dto: CrudApproverProfileApiDto,
): Promise<ApiResult<ApproverProfileOption>> {
  return fetchJson<ApiResult<ApproverProfileOption>>(
    `/ApproverProfile/${id}`,
    {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(dto),
    },
  )
}

export async function deleteApproverProfileApi(
  id: string,
): Promise<ApiResult<void>> {
  return fetchJson<ApiResult<void>>(`/ApproverProfile/${id}`, {
    method: 'DELETE',
  })
}

export async function loadAccounts(): Promise<ApiResult<AccountOption[]>> {
  return fetchJson<ApiResult<AccountOption[]>>('/Account')
}

export async function createAccountApi(
  dto: CreateAccountApiDto,
): Promise<ApiResult<AccountOption>> {
  return fetchJson<ApiResult<AccountOption>>('/Account', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(dto),
  })
}

export async function updateAccountApi(
  dto: UpdateAccountApiDto,
): Promise<ApiResult<AccountOption>> {
  return fetchJson<ApiResult<AccountOption>>('/Account', {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(dto),
  })
}

export async function deleteAccountApi(id: string): Promise<ApiResult<void>> {
  return fetchJson<ApiResult<void>>(`/Account/${id}`, {
    method: 'DELETE',
  })
}

export async function loginAccountApi(
  dto: LoginAccountApiDto,
): Promise<ApiResult<LoginAccountApiResponse>> {
  return fetchJson<ApiResult<LoginAccountApiResponse>>('/Account/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(dto),
  })
}

export async function loadRequests(): Promise<ApiResult<RequestDetailsApiDto[]>> {
  return fetchJson<ApiResult<RequestDetailsApiDto[]>>('/Request')
}

export async function loadRequestsFiltered(params: {
  requestTypeId?: string
  status?: string
  regionId?: string
}): Promise<ApiResult<FilteredRequestApiDto[]>> {
  const query = new URLSearchParams()

  if (params.requestTypeId) {
    query.set('requestTypeId', params.requestTypeId)
  }

  if (params.status) {
    query.set('status', params.status)
  }

  if (params.regionId) {
    query.set('regionId', params.regionId)
  }

  return fetchJson<ApiResult<FilteredRequestApiDto[]>>(
    `/Request/filtered${query.toString() ? `?${query.toString()}` : ''}`,
  )
}

export async function loadRequestDetails(
  id: string,
): Promise<ApiResult<RequestDetailsApiDto>> {
  return fetchJson<ApiResult<RequestDetailsApiDto>>(`/Request/${id}`)
}

export async function loadRequestComments(
  id: string,
): Promise<ApiResult<CommentApiDto[]>> {
  return fetchJson<ApiResult<CommentApiDto[]>>(`/Comment/request/${id}`)
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
