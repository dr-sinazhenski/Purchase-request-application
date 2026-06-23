import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'

import {
  loadAccounts,
  loadApproverProfiles,
  loadRegions,
  loadRoles,
  updateAccountApi,
} from '../../../api'
import { AdminView } from '../../../components/AdminView/AdminView'

jest.mock('../../../api', () => ({
  loadAccounts: jest.fn(),
  loadApproverProfiles: jest.fn(),
  loadRegions: jest.fn(),
  loadRoles: jest.fn(),
  updateAccountApi: jest.fn(),
}))

const accounts = [
  {
    id: 'account-1',
    login: 'john.doe',
    name: 'John Doe',
    regionId: 'region-eu',
    regionName: 'Europe',
    approverProfileId: undefined,
    approverProfileName: undefined,
    roleIds: ['role-requester'],
    roleNames: ['Requester'],
  },
  {
    id: 'account-2',
    login: 'jane.smith',
    name: 'Jane Smith',
    regionId: 'region-us',
    regionName: 'North America',
    approverProfileId: 'profile-1',
    approverProfileName: 'Approval queue',
    roleIds: ['role-approver'],
    roleNames: ['Approver'],
  },
]

const roles = [
  { id: 'role-requester', name: 'Requester' },
  { id: 'role-approver', name: 'Approver' },
  { id: 'role-admin', name: 'Admin' },
  { id: 'role-hidden', name: 'BackendOnly' },
]

const regions = [
  { id: 'region-eu', name: 'Europe', currency: 'EUR' },
  { id: 'region-us', name: 'North America', currency: 'USD' },
]

const profiles = [
  {
    id: 'profile-1',
    name: 'Approval queue',
    minAmount: 0,
    maxAmount: 5000,
  },
]

function renderAdmin(overrides: Partial<Parameters<typeof AdminView>[0]> = {}) {
  const props: Parameters<typeof AdminView>[0] = {
    onBackToUsers: jest.fn(),
    onOpenUser: jest.fn(),
    ...overrides,
  }

  return {
    props,
    user: userEvent.setup(),
    ...render(<AdminView {...props} />),
  }
}

describe('AdminView', () => {
  beforeEach(() => {
    jest.mocked(loadAccounts).mockResolvedValue({
      isSuccess: true,
      data: accounts,
    })
    jest.mocked(loadRoles).mockResolvedValue({ isSuccess: true, data: roles })
    jest.mocked(loadRegions).mockResolvedValue({
      isSuccess: true,
      data: regions,
    })
    jest.mocked(loadApproverProfiles).mockResolvedValue({
      isSuccess: true,
      data: profiles,
    })
    jest.mocked(updateAccountApi).mockResolvedValue({
      isSuccess: true,
      data: {
        ...accounts[1],
        roleIds: ['role-admin'],
        roleNames: ['Admin'],
        approverProfileId: undefined,
        approverProfileName: undefined,
      },
    })
  })

  afterEach(() => {
    jest.clearAllMocks()
  })

  it('loads and renders user list', async () => {
    renderAdmin()

    expect(screen.getByText('Loading users...')).toBeInTheDocument()
    expect(await screen.findByText('John Doe')).toBeInTheDocument()
    expect(screen.getByText('Jane Smith')).toBeInTheDocument()
    expect(screen.queryByText('BackendOnly')).not.toBeInTheDocument()
  })

  it('filters users and opens selected user', async () => {
    const { props, user } = renderAdmin()

    await screen.findByText('John Doe')
    await user.type(screen.getByPlaceholderText('Search users'), 'jane')

    expect(screen.queryByText('John Doe')).not.toBeInTheDocument()
    await user.click(screen.getByRole('button', { name: /Jane Smith/i }))

    expect(props.onOpenUser).toHaveBeenCalledWith('account-2')
  })

  it('shows an empty state when search has no matches', async () => {
    const { user } = renderAdmin()

    await screen.findByText('John Doe')
    await user.type(screen.getByPlaceholderText('Search users'), 'nobody')

    expect(screen.getByText('No users match your search.')).toBeInTheDocument()
  })

  it('renders selected user profile and navigates back', async () => {
    const { props, user } = renderAdmin({ selectedUserId: 'account-2' })

    expect(await screen.findByRole('heading', { name: 'Jane Smith' })).toBeInTheDocument()
    expect(screen.getByText('jane.smith')).toBeInTheDocument()
    expect(screen.getByText('Approval queue')).toBeInTheDocument()

    await user.click(screen.getByRole('button', { name: 'Users' }))
    await user.click(screen.getByRole('button', { name: 'Cancel' }))

    expect(props.onBackToUsers).toHaveBeenCalledTimes(2)
  })

  it('validates password before saving user changes', async () => {
    const { user } = renderAdmin({ selectedUserId: 'account-2' })

    await screen.findByRole('heading', { name: 'Jane Smith' })
    await user.click(screen.getByRole('button', { name: 'Save user' }))

    expect(
      screen.getByText('Enter a new password for this user to save account changes.'),
    ).toBeInTheDocument()
    expect(updateAccountApi).not.toHaveBeenCalled()
  })

  it('saves selected user access changes', async () => {
    const { user } = renderAdmin({ selectedUserId: 'account-2' })

    await screen.findByRole('heading', { name: 'Jane Smith' })
    const [roleSelect, regionSelect] = screen.getAllByRole('combobox')

    await user.selectOptions(roleSelect, 'role-admin')
    await user.selectOptions(regionSelect, 'region-eu')
    await user.type(
      screen.getByPlaceholderText("Sets this user's password"),
      'new-password',
    )
    await user.click(screen.getByRole('button', { name: 'Save user' }))

    await waitFor(() => {
      expect(updateAccountApi).toHaveBeenCalledWith({
        id: 'account-2',
        login: 'jane.smith',
        password: 'new-password',
        name: 'Jane Smith',
        regionId: 'region-eu',
        approverProfileId: null,
        roleIds: ['role-admin'],
      })
    })
    expect(screen.getByText('Current role').parentElement).toHaveTextContent(
      'Admin',
    )
  })

  it('shows load errors and missing selected users', async () => {
    jest.mocked(loadAccounts).mockRejectedValueOnce(new Error('Users API down'))

    renderAdmin()

    expect(await screen.findByText('Users API down')).toBeInTheDocument()

    jest.clearAllMocks()
    jest.mocked(loadAccounts).mockResolvedValueOnce({
      isSuccess: true,
      data: accounts,
    })
    jest.mocked(loadRoles).mockResolvedValueOnce({ isSuccess: true, data: roles })
    jest.mocked(loadRegions).mockResolvedValueOnce({
      isSuccess: true,
      data: regions,
    })
    jest.mocked(loadApproverProfiles).mockResolvedValueOnce({
      isSuccess: true,
      data: profiles,
    })

    renderAdmin({ selectedUserId: 'missing' })

    expect(await screen.findByText('User not found')).toBeInTheDocument()
  })
})
