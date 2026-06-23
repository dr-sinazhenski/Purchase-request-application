import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'

import {
  createRequestApi,
  loadAccounts,
  loadPrices,
  loadProducts,
  loadRegions,
  loadRequestComments,
  loadRequestDetails,
  loadRequestTypes,
  loadRequestsFiltered,
  loginAccountApi,
  rejectRequestApi,
  setAuthToken,
  updateRequestApi,
} from '../../api'
import App from '../../App'

jest.mock('../../api', () => ({
  approveRequestApi: jest.fn(),
  clearAuthToken: jest.fn(),
  createAccountApi: jest.fn(),
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
  loginAccountApi: jest.fn(),
  rejectRequestApi: jest.fn(),
  setAuthToken: jest.fn(),
  updateRequestApi: jest.fn(),
}))

const identityClaim =
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'

function makeToken(accountId: string, currency: string) {
  return `header.${window.btoa(
    JSON.stringify({ [identityClaim]: accountId, currency }),
  )}.signature`
}

const regions = [
  { id: 'region-eu', name: 'Europe', currency: 'EUR' },
  { id: 'region-us', name: 'North America', currency: 'USD' },
]

const requestTypes = [{ id: 'type-it', name: 'IT Products' }]

const products = [
  {
    id: 'product-monitor',
    name: 'Monitor',
    description: '24-inch display',
    requestTypeIds: ['type-it'],
  },
]

const prices = [
  {
    productId: 'product-monitor',
    regionId: 'region-eu',
    amount: 1000,
    unitsOfMeasure: 'pcs',
  },
  {
    productId: 'product-monitor',
    regionId: 'region-us',
    amount: 1188,
    unitsOfMeasure: 'pcs',
  },
]

const requesterAccount = {
  id: 'account-requester',
  login: 'john.doe',
  name: 'John Doe',
  regionId: 'region-us',
  regionName: 'North America',
  approverProfileId: undefined,
  approverProfileName: 'Approval queue',
  roleIds: ['role-requester'],
  roleNames: ['Requester'],
  token: makeToken('account-requester', 'USD'),
}

const adminAccount = {
  id: 'account-admin',
  login: 'admin',
  name: 'Alex Admin',
  regionId: 'region-eu',
  regionName: 'Europe',
  approverProfileId: undefined,
  approverProfileName: undefined,
  roleIds: ['role-admin'],
  roleNames: ['Admin'],
  token: makeToken('account-admin', 'EUR'),
}

const approverAccount = {
  id: 'account-approver',
  login: 'jane.smith',
  name: 'Jane Smith',
  regionId: 'region-us',
  regionName: 'North America',
  approverProfileId: 'profile-1',
  approverProfileName: 'Approval queue',
  roleIds: ['role-approver'],
  roleNames: ['Approver'],
  token: makeToken('account-approver', 'USD'),
}

const monitorRequest = {
  id: 'request-new',
  title: 'Monitor request',
  description: 'Need a display',
  requestType: requestTypes[0],
  status: 'Submited',
  rejectionCommentText: undefined,
  createdAt: '2026-06-23T10:00:00Z',
  updatedAt: '2026-06-23T10:00:00Z',
  products: [
    {
      id: 'product-monitor',
      name: 'Monitor',
      description: '24-inch display',
      amount: 1,
      price: 1000,
    },
  ],
}

const returnedRequest = {
  ...monitorRequest,
  id: 'request-returned',
  title: 'Returned monitor request',
  status: 'ForRevision',
  rejectionCommentText: 'Please clarify the monitor model.',
}

const requestDetailsById = {
  'request-new': monitorRequest,
  'request-returned': returnedRequest,
}

function mockBackend() {
  jest.mocked(loadRegions).mockResolvedValue({ isSuccess: true, data: regions })
  jest.mocked(loadAccounts).mockResolvedValue({
    isSuccess: true,
    data: [requesterAccount, adminAccount, approverAccount],
  })
  jest.mocked(loadRequestTypes).mockResolvedValue({
    isSuccess: true,
    data: requestTypes,
  })
  jest.mocked(loadProducts).mockResolvedValue({
    isSuccess: true,
    data: products,
  })
  jest.mocked(loadPrices).mockResolvedValue({
    isSuccess: true,
    data: prices,
  })
  jest.mocked(loadRequestsFiltered).mockResolvedValue({
    isSuccess: true,
    data: [
      {
        id: 'request-new',
        title: 'Monitor request',
        requestType: 'IT Products',
        status: 'Submited',
        totalPrice: 1000,
        createdAt: '2026-06-23T10:00:00Z',
        updatedAt: '2026-06-23T10:00:00Z',
      },
      {
        id: 'request-returned',
        title: 'Returned monitor request',
        requestType: 'IT Products',
        status: 'ForRevision',
        totalPrice: 1000,
        createdAt: '2026-06-23T10:00:00Z',
        updatedAt: '2026-06-23T10:00:00Z',
      },
    ],
  })
  jest.mocked(loadRequestDetails).mockImplementation(async (id: string) => ({
    isSuccess: true,
    data: requestDetailsById[id as keyof typeof requestDetailsById],
  }))
  jest.mocked(loadRequestComments).mockResolvedValue({
    isSuccess: true,
    data: [
      {
        id: 'comment-1',
        requestId: 'request-returned',
        text: 'Please clarify the monitor model.',
        creationTime: '2026-06-23T11:00:00Z',
      },
    ],
  })
  jest.mocked(loginAccountApi).mockResolvedValue({
    isSuccess: true,
    data: {
      token: adminAccount.token,
      name: adminAccount.name,
      roles: ['Admin'],
    },
  })
  jest.mocked(createRequestApi).mockResolvedValue({
    isSuccess: true,
    data: {
      id: 'request-created',
      title: 'Created monitor request',
      description: 'Need another display',
      requestType: requestTypes[0],
      status: 'Submited',
      createdAt: '2026-06-23T12:00:00Z',
      products: [],
    },
  })
  jest.mocked(updateRequestApi).mockImplementation(async () => {
    const updatedRequest = {
      id: 'request-returned',
      title: 'Updated returned request',
      description: 'Updated details',
      requestType: requestTypes[0],
      status: 'Resubmited',
      updatedAt: '2026-06-23T12:00:00Z',
      products: [],
    }

    requestDetailsById['request-returned'] = updatedRequest

    return {
      isSuccess: true,
      data: updatedRequest,
    }
  })
  jest.mocked(rejectRequestApi).mockResolvedValue({ isSuccess: true })
}

describe('App integration flows', () => {
  beforeEach(() => {
    window.localStorage.clear()
    window.history.pushState(null, '', '/')
    mockBackend()
  })

  afterEach(() => {
    jest.clearAllMocks()
    window.localStorage.clear()
  })

  it('signs in an admin and opens a request detail from the loaded list', async () => {
    const user = userEvent.setup()
    window.history.pushState(null, '', '/sign-in')

    render(<App />)

    await user.type(screen.getByLabelText(/Login/i), 'admin')
    await user.type(screen.getByPlaceholderText('Password'), 'hashed_password_4')
    await user.click(screen.getByRole('button', { name: 'Sign in' }))

    expect(
      await screen.findByRole('heading', { name: 'All Requests' }),
    ).toBeInTheDocument()
    expect(setAuthToken).toHaveBeenCalledWith(adminAccount.token)

    await user.click(screen.getByRole('button', { name: 'Monitor request' }))

    expect(
      await screen.findByRole('heading', { name: 'Monitor request' }),
    ).toBeInTheDocument()
    expect(screen.getByText('Need a display')).toBeInTheDocument()
    expect(screen.getAllByText('€1,000.00')[0]).toBeInTheDocument()
  })

  it('creates a requester-owned request using prices from the requester region', async () => {
    const user = userEvent.setup()
    jest.mocked(loadRequestsFiltered).mockResolvedValue({
      isSuccess: true,
      data: [
        {
          id: 'request-created',
          title: 'Created monitor request',
          requestType: 'IT Products',
          status: 'Submited',
          totalPrice: 1188,
          createdAt: '2026-06-23T12:00:00Z',
          updatedAt: '2026-06-23T12:00:00Z',
        },
      ],
    })
    jest.mocked(loadRequestDetails).mockImplementation(async (id: string) => {
      if (id === 'request-created') {
        return {
          isSuccess: true,
          data: {
            id: 'request-created',
            title: 'Created monitor request',
            description: 'Need another display',
            requestType: requestTypes[0],
            status: 'Submited',
            createdAt: '2026-06-23T12:00:00Z',
            updatedAt: '2026-06-23T12:00:00Z',
            products: [
              {
                id: 'product-monitor',
                name: 'Monitor',
                description: '24-inch display',
                amount: 1,
                price: 1188,
              },
            ],
          },
        }
      }

      return {
        isSuccess: true,
        data: requestDetailsById[id as keyof typeof requestDetailsById],
      }
    })
    window.localStorage.setItem(
      'procureflow.account',
      JSON.stringify(requesterAccount),
    )
    window.history.pushState(null, '', '/requests/new')

    render(<App />)

    expect(
      await screen.findByRole('heading', { name: 'Create Purchase Request' }),
    ).toBeInTheDocument()

    const textboxes = screen.getAllByRole('textbox')
    await user.type(textboxes[0], 'Created monitor request')
    await user.selectOptions(screen.getByLabelText('Product'), 'product-monitor')
    await user.type(textboxes[textboxes.length - 1], 'Need another display')

    expect(screen.getAllByText('$1,188.00')[0]).toBeInTheDocument()

    await user.click(screen.getByRole('button', { name: /Submit request/i }))

    await waitFor(() => {
      expect(createRequestApi).toHaveBeenCalledWith({
        title: 'Created monitor request',
        description: 'Need another display',
        requestTypeId: 'type-it',
        requesterId: 'account-requester',
        productIdAmount: { 'product-monitor': 1 },
      })
    })
    expect(
      await screen.findByRole('heading', { name: 'All Requests' }),
    ).toBeInTheDocument()
    expect(
      await screen.findByRole('button', { name: 'Created monitor request' }),
    ).toBeInTheDocument()
  })

  it('edits a returned request and resubmits it from the form', async () => {
    const user = userEvent.setup()
    window.localStorage.setItem(
      'procureflow.account',
      JSON.stringify(requesterAccount),
    )
    window.localStorage.setItem(
      'procureflow.requestOwners',
      JSON.stringify({ 'request-returned': requesterAccount.id }),
    )
    window.history.pushState(null, '', '/requests')

    render(<App />)

    expect(
      await screen.findByRole('heading', { name: 'All Requests' }),
    ).toBeInTheDocument()
    await user.click(
      await screen.findByRole('button', { name: 'Returned monitor request' }),
    )
    await user.click(await screen.findByRole('button', { name: 'Edit request' }))

    expect(
      await screen.findByRole('heading', {
        level: 2,
        name: 'Edit Purchase Request',
      }),
    ).toBeInTheDocument()
    expect(screen.getByText('Please clarify the monitor model.')).toBeInTheDocument()

    const nameInput = screen.getByDisplayValue('Returned monitor request')
    const detailsInput = screen.getByDisplayValue('Need a display')
    await user.clear(nameInput)
    await user.type(nameInput, 'Updated returned request')
    await user.clear(detailsInput)
    await user.type(detailsInput, 'Updated details')
    await user.click(screen.getByRole('button', { name: /Save changes/i }))

    await waitFor(() => {
      expect(updateRequestApi).toHaveBeenCalledWith({
        id: 'request-returned',
        title: 'Updated returned request',
        description: 'Updated details',
        requestTypeId: 'type-it',
        requesterId: 'account-requester',
        productIdAmount: { 'product-monitor': 1 },
      })
    })
    expect(await screen.findByText('Resubmitted')).toBeInTheDocument()
  })

  it('rejects a queued request as an approver and shows the final decision banner', async () => {
    const user = userEvent.setup()
    window.localStorage.setItem(
      'procureflow.account',
      JSON.stringify(approverAccount),
    )
    window.history.pushState(null, '', '/approval')

    render(<App />)

    expect(
      await screen.findByRole('heading', { name: 'Approval Queue' }),
    ).toBeInTheDocument()
    await user.click(await screen.findByRole('button', { name: /Monitor request/i }))
    await user.click(screen.getByRole('button', { name: 'Reject request' }))
    await user.type(
      screen.getByPlaceholderText('Provide a reason if rejecting...'),
      'Budget is not approved.',
    )
    await user.click(screen.getByRole('button', { name: 'Reject' }))

    await waitFor(() => {
      expect(rejectRequestApi).toHaveBeenCalledWith({
        id: 'request-new',
        reason: 'Budget is not approved.',
        isFinal: true,
      })
    })
    expect(await screen.findByText('Request rejected')).toBeInTheDocument()
  })
})
