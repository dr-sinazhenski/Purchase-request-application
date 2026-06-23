import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'

import {
  approveRequestApi,
  clearAuthToken,
  createRequestApi,
  deleteRequestApi,
  loadAccounts,
  loadPrices,
  loadProducts,
  loadRegions,
  loadRequestComments,
  loadRequestDetails,
  loadRequestTypes,
  loadRequestsFiltered,
  rejectRequestApi,
} from '../../api'
import App from '../../App'

jest.mock('../../api', () => ({
  approveRequestApi: jest.fn(),
  clearAuthToken: jest.fn(),
  createRequestApi: jest.fn(),
  deleteRequestApi: jest.fn(),
  loadAccounts: jest.fn(),
  loadPrices: jest.fn(),
  loadProducts: jest.fn(),
  loadRegions: jest.fn(),
  loadRequestComments: jest.fn(),
  loadRequestDetails: jest.fn(),
  loadRequestTypes: jest.fn(),
  loadRequestsFiltered: jest.fn(),
  rejectRequestApi: jest.fn(),
  updateRequestApi: jest.fn(),
}))

const account = {
  id: 'account-1',
  login: 'john.doe',
  name: 'John Doe',
  regionId: 'region-us',
  regionName: 'North America',
  approverProfileId: undefined,
  approverProfileName: undefined,
  roleIds: ['role-requester'],
  roleNames: ['Requester'],
  token: `header.${window.btoa(JSON.stringify({ currency: 'USD' }))}.signature`,
}

const requestDto = {
  id: 'request-1',
  title: 'Monitor request',
  description: 'Need display',
  requestType: { id: 'type-1', name: 'IT Products' },
  status: 'Submited',
  createdAt: '2026-06-23T10:00:00Z',
  updatedAt: '2026-06-23T10:00:00Z',
  products: [
    {
      id: 'product-1',
      name: 'Monitor',
      description: '24-inch display',
      amount: 1,
      price: 1188,
    },
  ],
}

function mockSuccessfulLoads() {
  jest.mocked(loadRegions).mockResolvedValue({
    isSuccess: true,
    data: [{ id: 'region-us', name: 'North America', currency: 'USD' }],
  })
  jest.mocked(loadAccounts).mockResolvedValue({
    isSuccess: true,
    data: [account],
  })
  jest.mocked(loadRequestsFiltered).mockResolvedValue({
    isSuccess: true,
    data: [
      {
        id: 'request-1',
        title: 'Monitor request',
        requestType: 'IT Products',
        status: 'Submited',
        totalPrice: 1188,
        createdAt: '2026-06-23T10:00:00Z',
        updatedAt: '2026-06-23T10:00:00Z',
      },
    ],
  })
  jest.mocked(loadRequestDetails).mockResolvedValue({
    isSuccess: true,
    data: requestDto,
  })
  jest.mocked(loadRequestComments).mockResolvedValue({
    isSuccess: true,
    data: [],
  })
  jest.mocked(loadRequestTypes).mockResolvedValue({
    isSuccess: true,
    data: [{ id: 'type-1', name: 'IT Products' }],
  })
  jest.mocked(loadProducts).mockResolvedValue({
    isSuccess: true,
    data: [
      {
        id: 'product-1',
        name: 'Monitor',
        description: '24-inch display',
        requestTypeIds: ['type-1'],
      },
    ],
  })
  jest.mocked(loadPrices).mockResolvedValue({
    isSuccess: true,
    data: [
      {
        productId: 'product-1',
        regionId: 'region-us',
        amount: 1188,
        unitsOfMeasure: 'pcs',
      },
    ],
  })
  jest.mocked(createRequestApi).mockResolvedValue({
    isSuccess: true,
    data: {
      id: 'created-1',
      title: 'Created monitor request',
      description: 'Need another display',
      requestType: { id: 'type-1', name: 'IT Products' },
      status: 'Submited',
      createdAt: '2026-06-23T10:00:00Z',
      updatedAt: '2026-06-23T10:00:00Z',
      products: [],
    },
  })
  jest.mocked(deleteRequestApi).mockResolvedValue({ isSuccess: true })
  jest.mocked(approveRequestApi).mockResolvedValue({ isSuccess: true })
  jest.mocked(rejectRequestApi).mockResolvedValue({ isSuccess: true })
}

describe('App', () => {
  beforeEach(() => {
    window.localStorage.clear()
    window.history.pushState(null, '', '/')
    mockSuccessfulLoads()
  })

  afterEach(() => {
    jest.clearAllMocks()
    window.localStorage.clear()
  })

  it('renders signin flow and opens privacy policy from signup', async () => {
    const user = userEvent.setup()

    render(<App />)

    expect(screen.getByRole('heading', { name: 'Welcome back' })).toBeInTheDocument()

    await user.click(screen.getByRole('button', { name: 'Create one' }))
    expect(screen.getByRole('heading', { name: 'Create your account' })).toBeInTheDocument()

    await user.click(screen.getByRole('button', { name: 'Privacy Policy' }))
    expect(screen.getByRole('heading', { name: 'Privacy Policy' })).toBeInTheDocument()
  })

  it('loads stored requester account and renders owned requests in account currency', async () => {
    window.localStorage.setItem('procureflow.account', JSON.stringify(account))
    window.localStorage.setItem(
      'procureflow.requestOwners',
      JSON.stringify({ 'request-1': 'account-1' }),
    )
    window.history.pushState(null, '', '/requests')

    render(<App />)

    expect(await screen.findByRole('heading', { name: 'All Requests' })).toBeInTheDocument()
    expect(await screen.findByRole('button', { name: 'Monitor request' })).toBeInTheDocument()
    expect(screen.getByText('$1,188.00')).toBeInTheDocument()
    expect(loadRequestsFiltered).toHaveBeenCalledWith({
      requestTypeId: undefined,
      status: undefined,
    })
  })

  it('applies request list search, status, type, and sort controls', async () => {
    const user = userEvent.setup()
    window.localStorage.setItem('procureflow.account', JSON.stringify(account))
    window.localStorage.setItem(
      'procureflow.requestOwners',
      JSON.stringify({ 'request-1': 'account-1' }),
    )
    window.history.pushState(null, '', '/requests')

    render(<App />)

    expect(await screen.findByRole('button', { name: 'Monitor request' })).toBeInTheDocument()

    await user.type(screen.getByPlaceholderText('Search requests'), 'monitor')
    await user.click(screen.getAllByRole('button', { name: 'Approved' })[0])
    await user.click(screen.getAllByRole('button', { name: 'IT Products' })[0])
    await user.click(screen.getAllByLabelText('Sort requests')[0])
    await user.click(screen.getAllByRole('button', { name: 'Price: low to high' })[0])

    await waitFor(() => {
      expect(loadRequestsFiltered).toHaveBeenCalledWith({
        requestTypeId: 'type-1',
        status: 'Approved',
      })
    })
    expect(screen.getByPlaceholderText('Search requests')).toHaveValue('monitor')
    expect(screen.getAllByText('Price: low to high')[0]).toBeInTheDocument()
  })

  it('logs out from the profile screen', async () => {
    const user = userEvent.setup()
    window.localStorage.setItem('procureflow.account', JSON.stringify(account))
    window.history.pushState(null, '', '/profile')

    render(<App />)

    await screen.findByRole('heading', { name: 'Profile & Settings' })
    await user.click(screen.getByRole('button', { name: 'Log out' }))

    expect(clearAuthToken).toHaveBeenCalledTimes(1)
    await waitFor(() => {
      expect(screen.getByRole('heading', { name: 'Welcome back' })).toBeInTheDocument()
    })
  })

  it('creates a request from the app shell', async () => {
    const user = userEvent.setup()
    window.localStorage.setItem('procureflow.account', JSON.stringify(account))
    window.localStorage.setItem(
      'procureflow.requestOwners',
      JSON.stringify({ 'request-1': 'account-1' }),
    )
    window.history.pushState(null, '', '/requests')

    render(<App />)

    await screen.findByRole('heading', { name: 'All Requests' })
    await user.click(screen.getByRole('button', { name: '+ New Request' }))
    expect(
      await screen.findByRole('heading', { name: 'Create Purchase Request' }),
    ).toBeInTheDocument()

    const textboxes = screen.getAllByRole('textbox')
    await user.type(textboxes[0], 'Created monitor request')
    await user.type(textboxes[textboxes.length - 1], 'Need another display')
    await user.selectOptions(screen.getByLabelText('Product'), 'product-1')
    await user.click(screen.getByRole('button', { name: /Submit request/i }))

    await waitFor(() => {
      expect(createRequestApi).toHaveBeenCalledWith({
        title: 'Created monitor request',
        description: 'Need another display',
        requestTypeId: 'type-1',
        requesterId: 'account-1',
        productIdAmount: { 'product-1': 1 },
      })
    })
    expect(
      await screen.findByRole('heading', { name: 'All Requests' }),
    ).toBeInTheDocument()
  })

  it('approves a queued request from approval view', async () => {
    const user = userEvent.setup()
    const approverAccount = {
      ...account,
      roleIds: ['role-approver'],
      roleNames: ['Approver'],
      approverProfileName: 'Approval queue',
    }
    window.localStorage.setItem(
      'procureflow.account',
      JSON.stringify(approverAccount),
    )
    window.history.pushState(null, '', '/approval')

    render(<App />)

    expect(
      await screen.findByRole('heading', { name: 'Approval Queue' }),
    ).toBeInTheDocument()
    const requestButton = await screen.findByRole('button', {
      name: /Monitor request/i,
    })
    await user.click(requestButton)
    await user.click(screen.getByRole('button', { name: 'Approve request' }))

    await waitFor(() => {
      expect(approveRequestApi).toHaveBeenCalledWith('request-1')
    })
    expect(await screen.findByText('Request approved')).toBeInTheDocument()
  })

  it('deletes an owned request from the detail screen', async () => {
    const user = userEvent.setup()
    const confirmSpy = jest.spyOn(window, 'confirm').mockReturnValue(true)
    window.localStorage.setItem('procureflow.account', JSON.stringify(account))
    window.localStorage.setItem(
      'procureflow.requestOwners',
      JSON.stringify({ 'request-1': 'account-1' }),
    )
    window.history.pushState(null, '', '/requests/request-1')

    render(<App />)

    expect(
      await screen.findByRole('heading', { name: 'Monitor request' }),
    ).toBeInTheDocument()
    await user.click(screen.getByRole('button', { name: /Delete/i }))

    await waitFor(() => {
      expect(deleteRequestApi).toHaveBeenCalledWith('request-1')
    })
    expect(confirmSpy).toHaveBeenCalledWith(
      'Delete request "Monitor request"? This cannot be undone.',
    )
    expect(
      await screen.findByRole('heading', { name: 'All Requests' }),
    ).toBeInTheDocument()

    confirmSpy.mockRestore()
  })

  it('returns a selected approval request for revision', async () => {
    const user = userEvent.setup()
    const approverAccount = {
      ...account,
      roleIds: ['role-approver'],
      roleNames: ['Approver'],
      approverProfileName: 'Approval queue',
    }
    window.localStorage.setItem(
      'procureflow.account',
      JSON.stringify(approverAccount),
    )
    window.history.pushState(null, '', '/requests/request-1/approval')

    render(<App />)

    expect(
      await screen.findByRole('heading', { name: 'Monitor request' }),
    ).toBeInTheDocument()
    await user.click(screen.getByRole('button', { name: 'Return for revision' }))
    await user.type(
      screen.getByPlaceholderText('Provide a reason for returning the request...'),
      'Please clarify the monitor model.',
    )
    await user.click(screen.getByRole('button', { name: 'Return' }))

    await waitFor(() => {
      expect(rejectRequestApi).toHaveBeenCalledWith({
        id: 'request-1',
        reason: 'Please clarify the monitor model.',
        isFinal: false,
      })
    })
    expect(
      await screen.findByText('Request returned for revision'),
    ).toBeInTheDocument()
  })
})
