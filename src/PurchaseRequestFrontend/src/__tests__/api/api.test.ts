import {
  approveRequestApi,
  clearAuthToken,
  createAccountApi,
  createApproverProfileApi,
  createRequestApi,
  createRoleApi,
  deleteAccountApi,
  deleteApproverProfileApi,
  deleteRequestApi,
  deleteRoleApi,
  hasStoredAuthToken,
  loadAccounts,
  loadApproverProfiles,
  loadPrices,
  loadProducts,
  loadRegions,
  loadRequestComments,
  loadRequestDetails,
  loadRequests,
  loadRequestsFiltered,
  loadRequestTypes,
  loadRoles,
  loginAccountApi,
  rejectRequestApi,
  setAuthToken,
  updateAccountApi,
  updateApproverProfileApi,
  updateRequestApi,
  updateRoleApi,
} from '../../api'

function mockFetchJson(payload: unknown, ok = true, statusText = 'OK') {
  const fetchMock = jest.fn().mockResolvedValue({
    ok,
    statusText,
    text: jest.fn().mockResolvedValue(JSON.stringify(payload)),
  })

  global.fetch = fetchMock

  return fetchMock
}

function getFetchInit(fetchMock: jest.Mock, callIndex = 0) {
  return fetchMock.mock.calls[callIndex][1] as RequestInit
}

describe('api client', () => {
  beforeEach(() => {
    window.localStorage.clear()
    mockFetchJson({ isSuccess: true, data: [] })
  })

  afterEach(() => {
    jest.clearAllMocks()
    window.localStorage.clear()
  })

  it('stores, reads, and clears auth tokens', () => {
    expect(hasStoredAuthToken()).toBe(false)

    setAuthToken('token-1')
    expect(hasStoredAuthToken()).toBe(true)

    clearAuthToken()
    expect(hasStoredAuthToken()).toBe(false)
  })

  it('uses token from stored account when token storage is empty', async () => {
    window.localStorage.setItem(
      'procureflow.account',
      JSON.stringify({ token: 'account-token' }),
    )
    const fetchMock = mockFetchJson({ isSuccess: true, data: [] })

    await loadRequestTypes()

    const headers = getFetchInit(fetchMock).headers as Headers
    expect(headers.get('Authorization')).toBe('Bearer account-token')
  })

  it('adds authorization headers and preserves explicit authorization', async () => {
    setAuthToken('stored-token')
    const fetchMock = mockFetchJson({ isSuccess: true, data: [] })

    await loadProducts()
    await loadPrices()
    await loadRegions()
    await loadRoles()
    await loadApproverProfiles()
    await loadAccounts()
    await loadRequests()

    expect(fetchMock).toHaveBeenNthCalledWith(1, '/Product', expect.any(Object))
    expect(fetchMock).toHaveBeenNthCalledWith(2, '/Price', expect.any(Object))
    expect(fetchMock).toHaveBeenNthCalledWith(3, '/Region', expect.any(Object))
    expect(fetchMock).toHaveBeenNthCalledWith(4, '/Role', expect.any(Object))
    expect(fetchMock).toHaveBeenNthCalledWith(5, '/ApproverProfile', expect.any(Object))
    expect(fetchMock).toHaveBeenNthCalledWith(6, '/Account', expect.any(Object))
    expect(fetchMock).toHaveBeenNthCalledWith(7, '/Request', expect.any(Object))

    const firstHeaders = getFetchInit(fetchMock).headers as Headers
    expect(firstHeaders.get('Authorization')).toBe('Bearer stored-token')
  })

  it('builds filtered request query strings', async () => {
    const fetchMock = mockFetchJson({ isSuccess: true, data: [] })

    await loadRequestsFiltered({
      requestTypeId: 'type-1',
      status: 'Submited',
      regionId: 'region-1',
    })
    await loadRequestsFiltered({})

    expect(fetchMock).toHaveBeenNthCalledWith(
      1,
      '/Request/filtered?requestTypeId=type-1&status=Submited&regionId=region-1',
      expect.any(Object),
    )
    expect(fetchMock).toHaveBeenNthCalledWith(
      2,
      '/Request/filtered',
      expect.any(Object),
    )
  })

  it('sends JSON bodies for create and update endpoints', async () => {
    const fetchMock = mockFetchJson({ isSuccess: true, data: { id: '1' } })

    await createRoleApi({ name: 'Requester' })
    await updateRoleApi('role-1', { name: 'Approver' })
    await createApproverProfileApi({
      name: 'Approval queue',
      minAmount: 0,
      maxAmount: 1000,
    })
    await updateApproverProfileApi('profile-1', {
      name: 'Large queue',
      minAmount: 1000,
      maxAmount: 5000,
    })
    await createAccountApi({
      login: 'john.doe',
      password: 'secret',
      name: 'John Doe',
      regionId: 'region-1',
      approverProfileId: null,
      roleIds: ['role-1'],
    })
    await updateAccountApi({
      id: 'account-1',
      login: 'john.doe',
      password: 'secret',
      name: 'John Doe',
      regionId: 'region-1',
      approverProfileId: null,
      roleIds: ['role-1'],
    })
    await loginAccountApi({ login: 'john.doe', password: 'secret' })
    await createRequestApi({
      title: 'Laptop',
      description: 'Need laptop',
      requestTypeId: 'type-1',
      requesterId: 'account-1',
      productIdAmount: { 'product-1': 2 },
    })
    await updateRequestApi({
      id: 'request-1',
      title: 'Laptop',
      description: 'Need laptop',
      requestTypeId: 'type-1',
      requesterId: 'account-1',
      productIdAmount: { 'product-1': 1 },
    })
    await rejectRequestApi({
      id: 'request-1',
      reason: 'Out of budget',
      isFinal: true,
    })

    expect(getFetchInit(fetchMock, 0)).toMatchObject({
      method: 'POST',
      body: JSON.stringify({ name: 'Requester' }),
    })
    expect(fetchMock).toHaveBeenNthCalledWith(
      2,
      '/Role/role-1',
      expect.objectContaining({ method: 'PUT' }),
    )
    expect(fetchMock).toHaveBeenNthCalledWith(
      10,
      '/Reject/',
      expect.objectContaining({
        method: 'PUT',
        body: JSON.stringify({
          id: 'request-1',
          reason: 'Out of budget',
          isFinal: true,
        }),
      }),
    )
  })

  it('calls delete, approve, details, and comments endpoints', async () => {
    const fetchMock = mockFetchJson({ isSuccess: true })

    await deleteRoleApi('role-1')
    await deleteApproverProfileApi('profile-1')
    await deleteAccountApi('account-1')
    await loadRequestDetails('request-1')
    await loadRequestComments('request-1')
    await deleteRequestApi('request-1')
    await approveRequestApi('request-1')

    expect(fetchMock).toHaveBeenNthCalledWith(
      1,
      '/Role/role-1',
      expect.objectContaining({ method: 'DELETE' }),
    )
    expect(fetchMock).toHaveBeenNthCalledWith(
      2,
      '/ApproverProfile/profile-1',
      expect.objectContaining({ method: 'DELETE' }),
    )
    expect(fetchMock).toHaveBeenNthCalledWith(
      3,
      '/Account/account-1',
      expect.objectContaining({ method: 'DELETE' }),
    )
    expect(fetchMock).toHaveBeenNthCalledWith(4, '/Request/request-1', expect.any(Object))
    expect(fetchMock).toHaveBeenNthCalledWith(5, '/Comment/request/request-1', expect.any(Object))
    expect(fetchMock).toHaveBeenNthCalledWith(
      6,
      '/Request/request-1',
      expect.objectContaining({ method: 'DELETE' }),
    )
    expect(fetchMock).toHaveBeenNthCalledWith(
      7,
      '/Approve/request-1',
      expect.objectContaining({ method: 'PUT' }),
    )
  })

  it('throws backend errors for non-ok responses', async () => {
    mockFetchJson(
      { error: { message: 'Backend validation failed' } },
      false,
      'Bad Request',
    )

    await expect(loadRegions()).rejects.toThrow('Backend validation failed')
  })

  it('returns success payload for empty responses', async () => {
    const fetchMock = jest.fn().mockResolvedValue({
      ok: true,
      statusText: 'OK',
      text: jest.fn().mockResolvedValue(''),
    })
    global.fetch = fetchMock

    await expect(deleteRequestApi('request-1')).resolves.toEqual({
      isSuccess: true,
    })
  })
})
