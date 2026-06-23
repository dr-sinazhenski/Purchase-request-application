import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'

import type { RequestRecord } from '../../../types'
import { RequestDetail } from '../../../components/RequestDetail/RequestDetail'

const request: RequestRecord = {
  id: 'request-1',
  name: 'Laptop request',
  type: 'IT Products',
  typeId: 'type-1',
  status: 'For Revision',
  total: 1200,
  creator: 'John Doe',
  initials: 'JD',
  updated: 'Just now',
  submitted: '23.06.2026',
  approver: 'Approval queue',
  description: 'Developer laptop',
  reason: 'Add missing invoice',
  ownerAccountId: 'account-1',
  items: [
    {
      name: 'Laptop',
      category: 'Standard business laptop',
      quantity: 2,
      unitPrice: 600,
      productId: 'product-1',
    },
  ],
}

function renderDetail(overrides: Partial<Parameters<typeof RequestDetail>[0]> = {}) {
  const props: Parameters<typeof RequestDetail>[0] = {
    canDelete: true,
    canEdit: true,
    canReview: true,
    currency: 'USD',
    onApprove: jest.fn(),
    onBack: jest.fn(),
    onDelete: jest.fn().mockResolvedValue(undefined),
    onEdit: jest.fn(),
    request,
    ...overrides,
  }

  return {
    props,
    user: userEvent.setup(),
    ...render(<RequestDetail {...props} />),
  }
}

describe('RequestDetail', () => {
  beforeEach(() => {
    jest.spyOn(window, 'confirm').mockReturnValue(true)
  })

  afterEach(() => {
    jest.restoreAllMocks()
  })

  it('renders request details, reason, and money in the provided currency', () => {
    renderDetail()

    expect(screen.getByRole('heading', { name: 'Laptop request' })).toBeInTheDocument()
    expect(screen.getByText('Add missing invoice')).toBeInTheDocument()
    expect(screen.getAllByText('$1,200.00')).toHaveLength(2)
    expect(screen.getByText('Standard business laptop · Qty 2')).toBeInTheDocument()
    expect(screen.getByText('Returned for revision')).toBeInTheDocument()
  })

  it('calls navigation and review handlers', async () => {
    const { props, user } = renderDetail({ request: { ...request, status: 'New', reason: undefined } })

    await user.click(screen.getByRole('button', { name: '← All Requests' }))
    await user.click(screen.getByRole('button', { name: 'Edit' }))
    await user.click(screen.getByRole('button', { name: 'Review' }))

    expect(props.onBack).toHaveBeenCalledTimes(1)
    expect(props.onEdit).toHaveBeenCalledTimes(1)
    expect(props.onApprove).toHaveBeenCalledTimes(1)
  })

  it('uses the edit request action for returned requests', async () => {
    const { props, user } = renderDetail()

    await user.click(screen.getByRole('button', { name: 'Edit request' }))

    expect(props.onEdit).toHaveBeenCalledTimes(1)
  })

  it('confirms before deleting and shows delete errors', async () => {
    const deleteError = new Error('Delete failed on backend')
    const { props, user } = renderDetail({
      onDelete: jest.fn().mockRejectedValue(deleteError),
    })

    await user.click(screen.getByRole('button', { name: /Delete/i }))

    expect(window.confirm).toHaveBeenCalledWith(
      'Delete request "Laptop request"? This cannot be undone.',
    )
    expect(props.onDelete).toHaveBeenCalledWith('request-1')
    expect(await screen.findByText('Delete failed')).toBeInTheDocument()
    expect(screen.getByText('Delete failed on backend')).toBeInTheDocument()
  })

  it('does not delete when confirmation is cancelled', async () => {
    jest.mocked(window.confirm).mockReturnValue(false)
    const { props, user } = renderDetail()

    await user.click(screen.getByRole('button', { name: /Delete/i }))

    await waitFor(() => {
      expect(props.onDelete).not.toHaveBeenCalled()
    })
  })

  it('disables actions after a final decision', () => {
    renderDetail({
      canDelete: true,
      request: { ...request, status: 'Approved', reason: undefined },
    })

    expect(
      screen.getByRole('button', { name: 'Final decision was made' }),
    ).toBeDisabled()
    expect(screen.queryByRole('button', { name: /Delete/i })).not.toBeInTheDocument()
  })
})
