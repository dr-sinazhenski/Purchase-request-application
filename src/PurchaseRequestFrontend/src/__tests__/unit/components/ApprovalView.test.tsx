import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'

import type { DecisionState, RequestRecord } from '../../../types'
import { ApprovalView } from '../../../components/ApprovalView/ApprovalView'

const request: RequestRecord = {
  id: 'request-1',
  name: 'Laptop request',
  type: 'IT Products',
  typeId: 'type-1',
  status: 'New',
  total: 1200,
  creator: 'John Doe',
  initials: 'JD',
  updated: 'Just now',
  submitted: '23.06.2026',
  approver: 'Approval queue',
  description: 'Developer laptop',
  ownerAccountId: 'account-1',
  items: [
    {
      name: 'Laptop',
      category: 'Standard business laptop',
      quantity: 1,
      unitPrice: 1200,
      productId: 'product-1',
    },
  ],
}

function renderApproval(overrides: Partial<Parameters<typeof ApprovalView>[0]> = {}) {
  const props: Parameters<typeof ApprovalView>[0] = {
    currency: 'EUR',
    decision: 'idle' as DecisionState,
    onBack: jest.fn(),
    onDecide: jest.fn().mockResolvedValue(undefined),
    onOpenRequest: jest.fn(),
    request,
    requests: [request],
    ...overrides,
  }

  return {
    props,
    user: userEvent.setup(),
    ...render(<ApprovalView {...props} />),
  }
}

describe('ApprovalView', () => {
  it('renders approval queue list, filters by search, and opens a request', async () => {
    const { props, user } = renderApproval({ request: undefined })

    expect(screen.getByText('Showing 1 of 1')).toBeInTheDocument()
    expect(screen.getByText(/€1,200\.00/)).toBeInTheDocument()

    await user.type(screen.getByLabelText('Search requests'), 'missing')
    expect(screen.getByText('No requests match your search.')).toBeInTheDocument()

    await user.clear(screen.getByLabelText('Search requests'))
    await user.click(screen.getByRole('button', { name: /Laptop request/i }))

    expect(props.onOpenRequest).toHaveBeenCalledWith(request)
  })

  it('renders an empty queue message when nothing is waiting for approval', () => {
    renderApproval({ request: undefined, requests: [] })

    expect(
      screen.getByText('No requests are waiting for approval.'),
    ).toBeInTheDocument()
  })

  it('approves immediately', async () => {
    const { props, user } = renderApproval()

    await user.click(screen.getByRole('button', { name: 'Approve request' }))

    expect(props.onDecide).toHaveBeenCalledWith('approved', '', false)
  })

  it('returns request for revision with a reason', async () => {
    const { props, user } = renderApproval()

    await user.click(screen.getByRole('button', { name: 'Return for revision' }))
    expect(screen.getByRole('dialog', { name: 'Return for revision' })).toBeInTheDocument()

    await user.type(
      screen.getByPlaceholderText('Provide a reason for returning the request...'),
      'Need invoice',
    )
    await user.click(screen.getByRole('button', { name: 'Return' }))

    expect(props.onDecide).toHaveBeenCalledWith(
      'rejected',
      'Need invoice',
      false,
    )
  })

  it('rejects request finally with a reason', async () => {
    const { props, user } = renderApproval()

    await user.click(screen.getByRole('button', { name: 'Reject request' }))
    expect(screen.getByRole('dialog', { name: 'Reject request' })).toBeInTheDocument()

    await user.type(
      screen.getByPlaceholderText('Provide a reason if rejecting...'),
      'Out of budget',
    )
    await user.click(screen.getByRole('button', { name: 'Reject' }))

    expect(props.onDecide).toHaveBeenCalledWith(
      'rejected',
      'Out of budget',
      true,
    )
  })

  it('closes decision dialogs with cancel', async () => {
    const { user } = renderApproval()

    await user.click(screen.getByRole('button', { name: 'Reject request' }))
    await user.type(
      screen.getByPlaceholderText('Provide a reason if rejecting...'),
      'Out of budget',
    )
    await user.click(screen.getByRole('button', { name: 'Cancel' }))

    expect(screen.queryByRole('dialog')).not.toBeInTheDocument()
  })

  it('shows recorded decision notices and hides decision buttons', () => {
    renderApproval({
      decision: 'returned',
      request: { ...request, status: 'For Revision' },
    })

    expect(screen.getByText('Request returned for revision')).toBeInTheDocument()
    expect(screen.queryByRole('button', { name: 'Approve request' })).not.toBeInTheDocument()
  })

  it('shows decision submission errors', async () => {
    const { user } = renderApproval({
      onDecide: jest.fn().mockRejectedValue(new Error('Backend unavailable')),
    })

    await user.click(screen.getByRole('button', { name: 'Approve request' }))

    expect(await screen.findByText('Decision failed')).toBeInTheDocument()
    expect(screen.getByText('Backend unavailable')).toBeInTheDocument()
  })
})
