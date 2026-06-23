import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'

import type { RequestRecord } from '../../../types'
import { RequestsList } from '../../../components/RequestsList/RequestsList'

const baseRequest: RequestRecord = {
  id: 'request-1',
  name: 'Laptop request',
  type: 'IT Products',
  typeId: 'type-1',
  status: 'New',
  total: 1200,
  creator: 'John Doe',
  initials: 'JD',
  updated: 'Just now',
  description: 'Developer laptop',
  approver: 'Approval queue',
  submitted: '23.06.2026',
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

function renderRequestsList(
  overrides: Partial<Parameters<typeof RequestsList>[0]> = {},
) {
  const props: Parameters<typeof RequestsList>[0] = {
    canCreateRequests: true,
    canReviewRequests: false,
    currency: 'USD',
    filter: 'All',
    filteredRequests: [baseRequest],
    onCreate: jest.fn(),
    onFilter: jest.fn(),
    onOpen: jest.fn(),
    onSearch: jest.fn(),
    onShowMore: jest.fn(),
    onSort: jest.fn(),
    onTypeFilter: jest.fn(),
    searchQuery: '',
    sort: 'newest',
    totalFiltered: 1,
    totalRequests: 1,
    typeFilter: 'All',
    uniqueTypes: ['IT Products'],
    visibleCount: 6,
    ...overrides,
  }

  return {
    props,
    user: userEvent.setup(),
    ...render(<RequestsList {...props} />),
  }
}

describe('RequestsList', () => {
  it('renders request rows and money in the provided currency', () => {
    renderRequestsList()

    expect(screen.getByRole('heading', { name: 'All Requests' })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Laptop request' })).toBeInTheDocument()
    expect(screen.getByText('$1,200.00')).toBeInTheDocument()
    expect(screen.getByText('John Doe')).toBeInTheDocument()
    expect(screen.getByText('Showing 1 of 1 filtered requests / 1 total')).toBeInTheDocument()
  })

  it('renders empty state', () => {
    renderRequestsList({ filteredRequests: [], totalFiltered: 0, totalRequests: 0 })

    expect(
      screen.getByText('No requests match your search or filters.'),
    ).toBeInTheDocument()
  })

  it('calls filter, type, search, sort, and create handlers', async () => {
    const { props, user } = renderRequestsList()

    await user.click(screen.getAllByRole('button', { name: 'Approved' })[0])
    await user.click(screen.getAllByRole('button', { name: 'IT Products' })[0])
    await user.click(screen.getByRole('button', { name: '+ New Request' }))
    await user.type(screen.getByLabelText('Search requests'), 'abc')
    await user.click(screen.getAllByRole('button', { name: 'Oldest first' })[0])

    expect(props.onFilter).toHaveBeenCalledWith('Approved')
    expect(props.onTypeFilter).toHaveBeenCalledWith('IT Products')
    expect(props.onCreate).toHaveBeenCalledTimes(1)
    expect(props.onSearch).toHaveBeenLastCalledWith('c')
    expect(props.onSort).toHaveBeenCalledWith('oldest')
  })

  it('opens request details from row name and action button', async () => {
    const { props, user } = renderRequestsList()

    await user.click(screen.getByRole('button', { name: 'Laptop request' }))
    await user.click(screen.getByRole('button', { name: 'View' }))

    expect(props.onOpen).toHaveBeenNthCalledWith(1, baseRequest)
    expect(props.onOpen).toHaveBeenNthCalledWith(2, baseRequest, 'detail')
  })

  it('routes new and resubmitted requests to review for approvers', async () => {
    const { props, user } = renderRequestsList({ canReviewRequests: true })

    await user.click(screen.getByRole('button', { name: 'Review' }))

    expect(props.onOpen).toHaveBeenCalledWith(baseRequest, 'approval')
  })

  it('shows show more when visible count is below total filtered', async () => {
    const { props, user } = renderRequestsList({
      totalFiltered: 4,
      totalRequests: 4,
      visibleCount: 1,
    })

    await user.click(screen.getByRole('button', { name: 'Show more' }))

    expect(props.onShowMore).toHaveBeenCalledTimes(1)
  })

  it('hides create action when user cannot create requests', () => {
    renderRequestsList({ canCreateRequests: false })

    expect(
      screen.queryByRole('button', { name: '+ New Request' }),
    ).not.toBeInTheDocument()
  })
})
