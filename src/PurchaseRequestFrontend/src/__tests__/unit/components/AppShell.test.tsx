import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'

import type { AccountOption } from '../../../api'
import { AppShell } from '../../../components/AppShell/AppShell'

const account: AccountOption = {
  id: 'account-1',
  login: 'john.doe',
  name: 'John Doe',
  regionId: 'region-1',
  regionName: 'North America',
  roleIds: ['role-1', 'role-2'],
  roleNames: ['Requester', 'Approver'],
}

function renderShell(overrides: Partial<Parameters<typeof AppShell>[0]> = {}) {
  const props: Parameters<typeof AppShell>[0] = {
    account,
    canCreateRequests: true,
    canManageAdmin: true,
    canReviewRequests: true,
    children: <div>Page content</div>,
    onAdmin: jest.fn(),
    onApprovalQueue: jest.fn(),
    onCreate: jest.fn(),
    onProfile: jest.fn(),
    onRequests: jest.fn(),
    reviewCount: 2,
    screen: 'requests',
    ...overrides,
  }

  return {
    props,
    user: userEvent.setup(),
    ...render(<AppShell {...props} />),
  }
}

describe('AppShell', () => {
  it('renders navigation and page content', () => {
    renderShell()

    expect(screen.getByText('ProcureFlow')).toBeInTheDocument()
    expect(screen.getByText('Page content')).toBeInTheDocument()
    expect(screen.getAllByText('All Requests')).toHaveLength(1)
    expect(screen.getByText('Requests')).toBeInTheDocument()
    expect(screen.getAllByText('Approval Queue')).toHaveLength(1)
    expect(screen.getAllByText('New Request')).toHaveLength(1)
    expect(screen.getAllByText('Admin')).toHaveLength(2)
  })

  it('calls desktop navigation handlers', async () => {
    const { props, user } = renderShell()

    await user.click(screen.getByRole('button', { name: /ProcureFlow/i }))
    await user.click(screen.getByRole('button', { name: /^All Requests$/i }))
    await user.click(screen.getByRole('button', { name: /Approval Queue/i }))
    await user.click(screen.getByRole('button', { name: /^New Request$/i }))
    await user.click(screen.getAllByRole('button', { name: /^Admin$/i })[0])

    expect(props.onRequests).toHaveBeenCalledTimes(2)
    expect(props.onApprovalQueue).toHaveBeenCalledTimes(1)
    expect(props.onCreate).toHaveBeenCalledTimes(1)
    expect(props.onAdmin).toHaveBeenCalledTimes(1)
  })

  it('shows avatar button and opens profile', async () => {
    const { props, user } = renderShell()

    const profileButton = screen.getByRole('button', {
      name: 'Open profile for John Doe',
    })

    expect(profileButton).toHaveTextContent('JD')

    await user.hover(profileButton)
    expect(screen.getByRole('tooltip')).toHaveTextContent('John Doe')
    expect(screen.getByRole('tooltip')).toHaveTextContent('Approver')

    await user.click(profileButton)
    expect(props.onProfile).toHaveBeenCalledTimes(1)
  })

  it('hides avatar button on profile screen', () => {
    renderShell({ screen: 'profile' })

    expect(
      screen.queryByRole('button', { name: 'Open profile for John Doe' }),
    ).not.toBeInTheDocument()
  })

  it('hides role-specific navigation when permissions are false', () => {
    renderShell({
      canCreateRequests: false,
      canManageAdmin: false,
      canReviewRequests: false,
    })

    expect(screen.queryByRole('button', { name: /Approval Queue/i })).not.toBeInTheDocument()
    expect(screen.queryByRole('button', { name: /^New Request$/i })).not.toBeInTheDocument()
    expect(screen.queryByRole('button', { name: /^Admin$/i })).not.toBeInTheDocument()
  })
})
