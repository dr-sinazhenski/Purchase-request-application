import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'

import {
  createRequestApi,
  loadPrices,
  loadProducts,
  loadRegions,
  loadRequestTypes,
  updateRequestApi,
} from '../../../api'
import type { AccountOption } from '../../../api'
import type { RequestRecord } from '../../../types'
import { RequestForm } from '../../../components/RequestForm/RequestForm'

jest.mock('../../../api', () => ({
  createRequestApi: jest.fn(),
  loadPrices: jest.fn(),
  loadProducts: jest.fn(),
  loadRegions: jest.fn(),
  loadRequestTypes: jest.fn(),
  updateRequestApi: jest.fn(),
}))

const requestType = { id: 'type-1', name: 'IT Products' }
const product = {
  id: 'product-1',
  name: 'Monitor',
  description: '24-inch display',
  requestTypeIds: ['type-1'],
}
const regions = [
  { id: 'region-eu', name: 'Europe', currency: 'EUR' },
  { id: 'region-us', name: 'North America', currency: 'USD' },
]
const prices = [
  {
    productId: 'product-1',
    regionId: 'region-eu',
    amount: 1000,
    unitsOfMeasure: 'pcs',
  },
  {
    productId: 'product-1',
    regionId: 'region-us',
    amount: 1188,
    unitsOfMeasure: 'pcs',
  },
]

const account: AccountOption = {
  id: 'account-1',
  login: 'john.doe',
  name: 'John Doe',
  regionId: 'region-us',
  regionName: 'North America',
  approverProfileName: 'Approval queue',
  roleIds: ['role-requester'],
  roleNames: ['Requester'],
  token: `header.${window.btoa(JSON.stringify({ currency: 'USD' }))}.signature`,
}

const baseRequest: RequestRecord = {
  id: 'draft-1',
  name: '',
  type: 'IT Products',
  typeId: 'type-1',
  status: 'New',
  total: 0,
  creator: 'John Doe',
  initials: 'JD',
  updated: 'Just now',
  submitted: '23.06.2026',
  approver: 'Approval queue',
  description: '',
  ownerAccountId: 'account-1',
  items: [
    {
      name: '',
      category: '',
      quantity: 1,
      unitPrice: 0,
      productId: undefined,
    },
  ],
}

function renderForm(overrides: Partial<Parameters<typeof RequestForm>[0]> = {}) {
  const props: Parameters<typeof RequestForm>[0] = {
    currentAccount: account,
    mode: 'create',
    onCancel: jest.fn(),
    onSubmit: jest.fn(),
    request: baseRequest,
    ...overrides,
  }

  return {
    props,
    user: userEvent.setup(),
    ...render(<RequestForm {...props} />),
  }
}

describe('RequestForm', () => {
  beforeEach(() => {
    jest.mocked(loadRequestTypes).mockResolvedValue({
      isSuccess: true,
      data: [requestType],
    })
    jest.mocked(loadProducts).mockResolvedValue({
      isSuccess: true,
      data: [product],
    })
    jest.mocked(loadPrices).mockResolvedValue({
      isSuccess: true,
      data: prices,
    })
    jest.mocked(loadRegions).mockResolvedValue({
      isSuccess: true,
      data: regions,
    })
    jest.mocked(createRequestApi).mockResolvedValue({
      isSuccess: true,
      data: {
        id: 'created-1',
        title: 'Monitor request',
        description: 'Need display',
        requestType,
        status: 'Submited',
        createdAt: '2026-06-23T10:00:00Z',
        products: [],
      },
    })
    jest.mocked(updateRequestApi).mockResolvedValue({
      isSuccess: true,
      data: {
        id: 'request-1',
        title: 'Updated monitor request',
        description: 'Need display',
        requestType,
        status: 'Resubmited',
        updatedAt: '2026-06-23T10:00:00Z',
        products: [],
      },
    })
  })

  afterEach(() => {
    jest.clearAllMocks()
  })

  it('loads backend options and prices products in the account currency', async () => {
    renderForm()

    await screen.findByRole('option', { name: 'Monitor' })
    await userEvent.selectOptions(screen.getByLabelText('Product'), 'product-1')

    expect(screen.getByLabelText('Unit of measure')).toHaveValue('pcs')
    expect(screen.getByLabelText('Unit price')).toHaveValue(1188)
    expect(screen.getAllByText('$1,188.00')).toHaveLength(2)
    expect(screen.getByText('USD')).toBeInTheDocument()
  })

  it('validates required fields before creating', async () => {
    const { user } = renderForm()

    await screen.findByRole('option', { name: 'Monitor' })
    await user.click(screen.getByRole('button', { name: /Submit request/i }))

    expect(screen.getByText('Please enter a request name.')).toBeInTheDocument()
    expect(createRequestApi).not.toHaveBeenCalled()
  })

  it('creates a request and maps backend response to frontend record', async () => {
    const { props, user } = renderForm()

    await screen.findByRole('option', { name: 'Monitor' })
    const textboxes = screen.getAllByRole('textbox')
    const nameInput = textboxes[0]
    const detailsInput = textboxes[textboxes.length - 1]

    await user.type(nameInput, 'Monitor request')
    await user.type(detailsInput, 'Need display')
    await user.selectOptions(screen.getByLabelText('Product'), 'product-1')
    await user.click(screen.getByRole('button', { name: /Submit request/i }))

    await waitFor(() => {
      expect(createRequestApi).toHaveBeenCalledWith({
        title: 'Monitor request',
        description: 'Need display',
        requestTypeId: 'type-1',
        requesterId: 'account-1',
        productIdAmount: { 'product-1': 1 },
      })
    })
    expect(props.onSubmit).toHaveBeenCalledWith(
      expect.objectContaining({
        id: 'created-1',
        name: 'Monitor request',
        status: 'New',
        total: 1188,
        creator: 'John Doe',
        approver: 'Approval queue',
      }),
    )
  })

  it('adds and removes item rows', async () => {
    const { user } = renderForm()

    await screen.findByRole('option', { name: 'Monitor' })
    await user.click(screen.getByRole('button', { name: 'Add product' }))
    expect(screen.getAllByLabelText('Product')).toHaveLength(2)

    await user.click(screen.getByRole('button', { name: 'Remove product 2' }))
    expect(screen.getAllByLabelText('Product')).toHaveLength(1)
  })

  it('updates returned requests and changes status to resubmitted', async () => {
    const { props, user } = renderForm({
      mode: 'edit',
      request: {
        ...baseRequest,
        id: 'request-1',
        name: 'Old monitor request',
        status: 'For Revision',
        reason: 'Fix quantity',
        items: [
          {
            name: 'Monitor',
            category: 'pcs',
            quantity: 1,
            unitPrice: 1188,
            productId: 'product-1',
          },
        ],
      },
    })

    await screen.findByRole('option', { name: 'Monitor' })
    const nameInput = screen.getByDisplayValue('Old monitor request')

    await user.clear(nameInput)
    await user.type(nameInput, 'Updated monitor request')
    await user.click(screen.getByRole('button', { name: /Save changes/i }))

    await waitFor(() => {
      expect(updateRequestApi).toHaveBeenCalledWith({
        id: 'request-1',
        title: 'Updated monitor request',
        description: '',
        requestTypeId: 'type-1',
        requesterId: 'account-1',
        productIdAmount: { 'product-1': 1 },
      })
    })
    expect(props.onSubmit).toHaveBeenCalledWith(
      expect.objectContaining({
        id: 'request-1',
        name: 'Updated monitor request',
        status: 'Resubmitted',
        total: 1188,
        reason: undefined,
      }),
    )
  })

  it('cancels without submitting', async () => {
    const { props, user } = renderForm()

    await user.click(screen.getByRole('button', { name: 'Cancel' }))

    expect(props.onCancel).toHaveBeenCalledTimes(1)
    expect(createRequestApi).not.toHaveBeenCalled()
  })

  it('shows API loading errors', async () => {
    jest.mocked(loadRequestTypes).mockResolvedValueOnce({
      isSuccess: false,
      error: { message: 'failed' },
    })

    renderForm()

    expect(
      await screen.findByText('Unable to load request types from API.'),
    ).toBeInTheDocument()
  })
})
