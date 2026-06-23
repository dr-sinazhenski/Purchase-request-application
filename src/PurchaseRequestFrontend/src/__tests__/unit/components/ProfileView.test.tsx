import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'

import {
  deleteAccountApi,
  loadApproverProfiles,
  loadRegions,
  loadRoles,
  updateAccountApi,
} from '../../../api'
import type { AccountOption } from '../../../api'
import { ProfileView } from '../../../components/ProfileView/ProfileView'

jest.mock('../../../api', () => ({
  deleteAccountApi: jest.fn(),
  loadApproverProfiles: jest.fn(),
  loadRegions: jest.fn(),
  loadRoles: jest.fn(),
  updateAccountApi: jest.fn(),
}))

const account: AccountOption = {
  id: 'account-1',
  login: 'john.doe',
  name: 'John Doe',
  regionId: 'region-us',
  regionName: 'North America',
  approverProfileId: 'profile-1',
  approverProfileName: 'Approval queue',
  roleIds: ['role-requester'],
  roleNames: ['Requester'],
}

const roles = [
  { id: 'role-requester', name: 'Requester' },
  { id: 'role-approver', name: 'Approver' },
]

const regions = [
  { id: 'region-eu', name: 'Europe', currency: 'EUR' },
  { id: 'region-us', name: 'North America', currency: 'USD' },
]

function renderProfile(overrides: Partial<Parameters<typeof ProfileView>[0]> = {}) {
  const props: Parameters<typeof ProfileView>[0] = {
    account,
    canManageRoles: false,
    onAccountChange: jest.fn(),
    onLogout: jest.fn(),
    ...overrides,
  }

  return {
    props,
    user: userEvent.setup(),
    ...render(<ProfileView {...props} />),
  }
}

describe('ProfileView', () => {
  beforeEach(() => {
    jest.spyOn(window, 'confirm').mockReturnValue(true)
    jest.mocked(loadRoles).mockResolvedValue({ isSuccess: true, data: roles })
    jest.mocked(loadRegions).mockResolvedValue({
      isSuccess: true,
      data: regions,
    })
    jest.mocked(loadApproverProfiles).mockResolvedValue({
      isSuccess: true,
      data: [
        {
          id: 'profile-1',
          name: 'Approval queue',
          minAmount: 0,
          maxAmount: 5000,
        },
      ],
    })
    jest.mocked(updateAccountApi).mockResolvedValue({
      isSuccess: true,
      data: { ...account, name: 'Johnny Doe' },
    })
    jest.mocked(deleteAccountApi).mockResolvedValue({ isSuccess: true })
  })

  afterEach(() => {
    jest.restoreAllMocks()
    jest.clearAllMocks()
  })

  it('renders account fields and readonly visible role', async () => {
    renderProfile()

    expect(screen.getByRole('heading', { name: 'Profile & Settings' })).toBeInTheDocument()
    expect(screen.getByDisplayValue('John')).toBeInTheDocument()
    expect(screen.getByDisplayValue('Doe')).toBeInTheDocument()
    expect(screen.getByDisplayValue('john.doe')).toBeInTheDocument()
    await screen.findByRole('option', { name: 'North America (USD)' })
    expect(screen.getByLabelText('Region')).toHaveValue('region-us')
    expect(screen.getByDisplayValue('Requester')).toBeInTheDocument()
  })

  it('requires password before saving profile changes', async () => {
    const { user } = renderProfile()

    await screen.findByRole('option', { name: 'North America (USD)' })
    await user.click(screen.getByRole('button', { name: 'Save changes' }))

    expect(screen.getByText('Password is required because the backend update needs it.')).toBeInTheDocument()
    expect(updateAccountApi).not.toHaveBeenCalled()
  })

  it('saves profile changes and reports success', async () => {
    const { props, user } = renderProfile()

    await screen.findByRole('option', { name: 'North America (USD)' })
    await user.clear(screen.getByLabelText('First name'))
    await user.type(screen.getByLabelText('First name'), 'Johnny')
    await user.type(screen.getByPlaceholderText('Required to save account changes'), 'secret')
    await user.click(screen.getByRole('button', { name: 'Save changes' }))

    await waitFor(() => {
      expect(updateAccountApi).toHaveBeenCalledWith({
        id: 'account-1',
        login: 'john.doe',
        password: 'secret',
        name: 'Johnny Doe',
        regionId: 'region-us',
        approverProfileId: null,
        roleIds: ['role-requester'],
      })
    })
    expect(props.onAccountChange).toHaveBeenCalledWith(
      expect.objectContaining({ name: 'Johnny Doe' }),
    )
    expect(screen.getByText('Profile saved.')).toBeInTheDocument()
  })

  it('lets admins manage roles and approver profile', async () => {
    const { user } = renderProfile({ canManageRoles: true })

    await screen.findByRole('option', { name: 'North America (USD)' })
    await user.click(screen.getByLabelText('Approver'))

    expect(screen.getByLabelText('Approver profile')).toBeInTheDocument()
    expect(
      screen.getByRole('option', { name: 'Approval queue (0-5000)' }),
    ).toBeInTheDocument()
  })

  it('deletes account after confirmation and logs out', async () => {
    const { props, user } = renderProfile()

    await user.click(screen.getByRole('button', { name: 'Delete account' }))

    expect(window.confirm).toHaveBeenCalledWith('Delete account "john.doe"?')
    expect(deleteAccountApi).toHaveBeenCalledWith('account-1')
    await waitFor(() => {
      expect(props.onLogout).toHaveBeenCalledTimes(1)
    })
  })

  it('does not delete when confirmation is cancelled', async () => {
    jest.mocked(window.confirm).mockReturnValue(false)
    const { props, user } = renderProfile()

    await user.click(screen.getByRole('button', { name: 'Delete account' }))

    expect(deleteAccountApi).not.toHaveBeenCalled()
    expect(props.onLogout).not.toHaveBeenCalled()
  })

  it('logs out from header action', async () => {
    const { props, user } = renderProfile()

    await user.click(screen.getByRole('button', { name: 'Log out' }))

    expect(props.onLogout).toHaveBeenCalledTimes(1)
  })
})
