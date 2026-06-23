import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'

import {
  createAccountApi,
  loadAccounts,
  loadRegions,
  loadRoles,
  loginAccountApi,
  setAuthToken,
} from '../../../api'
import { AuthView } from '../../../components/AuthView/AuthView'

jest.mock('../../../api', () => ({
  createAccountApi: jest.fn(),
  loadAccounts: jest.fn(),
  loadRegions: jest.fn(),
  loadRoles: jest.fn(),
  loginAccountApi: jest.fn(),
  setAuthToken: jest.fn(),
}))

const regions = [
  { id: 'region-eu', name: 'Europe', currency: 'EUR' },
  { id: 'region-us', name: 'North America', currency: 'USD' },
]

const roles = [
  { id: 'role-requester', name: 'Requester' },
  { id: 'role-admin', name: 'Admin' },
]

function makeToken(accountId = 'account-1') {
  const payload = window.btoa(
    JSON.stringify({
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier':
        accountId,
    }),
  )

  return `header.${payload}.signature`
}

function renderAuth(mode: 'signin' | 'signup' = 'signin') {
  const props: Parameters<typeof AuthView>[0] = {
    mode,
    onModeChange: jest.fn(),
    onPrivacyPolicy: jest.fn(),
    onSuccess: jest.fn(),
  }

  return {
    props,
    user: userEvent.setup(),
    ...render(<AuthView {...props} />),
  }
}

describe('AuthView', () => {
  beforeEach(() => {
    jest.mocked(loadRoles).mockResolvedValue({ isSuccess: true, data: roles })
    jest.mocked(loadRegions).mockResolvedValue({
      isSuccess: true,
      data: regions,
    })
    jest.mocked(loadAccounts).mockResolvedValue({ isSuccess: true, data: [] })
    jest.mocked(loginAccountApi).mockResolvedValue({
      isSuccess: true,
      data: {
        token: makeToken(),
        name: 'John Doe',
        roles: ['Requester'],
      },
    })
    jest.mocked(createAccountApi).mockResolvedValue({
      isSuccess: true,
      data: {
        id: 'account-2',
        login: 'new.user',
        name: 'New User',
        regionId: 'region-eu',
        regionName: 'Europe',
        roleIds: ['role-requester'],
        roleNames: ['Requester'],
      },
    })
  })

  afterEach(() => {
    jest.clearAllMocks()
  })

  it('validates required signin fields', async () => {
    const { user } = renderAuth()

    await user.click(screen.getByRole('button', { name: 'Sign in' }))

    expect(screen.getByText('Email is required.')).toBeInTheDocument()
    expect(screen.getByText('Password is required.')).toBeInTheDocument()
    expect(loginAccountApi).not.toHaveBeenCalled()
  })

  it('signs in and returns the decoded account id', async () => {
    const { props, user } = renderAuth()

    await user.type(screen.getByPlaceholderText('Login'), 'john.doe')
    await user.type(screen.getByPlaceholderText('Password'), 'secret')
    await user.click(screen.getByRole('button', { name: 'Sign in' }))

    expect(loginAccountApi).toHaveBeenCalledWith({
      login: 'john.doe',
      password: 'secret',
    })
    expect(setAuthToken).toHaveBeenCalledWith(makeToken())
    expect(props.onSuccess).toHaveBeenCalledWith(
      expect.objectContaining({
        id: 'account-1',
        login: 'john.doe',
        name: 'John Doe',
        roleNames: ['Requester'],
      }),
    )
  })

  it('loads signup options, opens privacy policy, and creates requester accounts', async () => {
    const { props, user } = renderAuth('signup')

    await screen.findByRole('option', { name: 'Europe (EUR)' })
    expect(screen.getByLabelText(/Region/i)).toHaveValue('region-eu')

    await user.type(screen.getByPlaceholderText('First name'), 'New')
    await user.type(screen.getByPlaceholderText('Last name'), 'User')
    await user.type(screen.getByPlaceholderText('Work email'), 'new.user')
    await user.type(screen.getByPlaceholderText('Password'), 'secret')
    await user.type(screen.getByPlaceholderText('Confirm password'), 'secret')

    await user.click(screen.getByRole('button', { name: 'Privacy Policy' }))
    expect(props.onPrivacyPolicy).toHaveBeenCalledTimes(1)

    await user.click(screen.getByLabelText(/I agree to the Terms/i))
    await user.click(screen.getByRole('button', { name: 'Create account' }))

    await waitFor(() => {
      expect(createAccountApi).toHaveBeenCalledWith({
        login: 'new.user',
        password: 'secret',
        name: 'New User',
        regionId: 'region-eu',
        approverProfileId: null,
        roleIds: ['role-requester'],
      })
    })
    expect(props.onSuccess).toHaveBeenCalledWith(
      expect.objectContaining({ id: 'account-2' }),
    )
  })

  it('shows signup validation errors', async () => {
    const { user } = renderAuth('signup')

    await screen.findByRole('option', { name: 'Europe (EUR)' })
    await user.click(screen.getByRole('button', { name: 'Create account' }))

    expect(screen.getByText('First name is required.')).toBeInTheDocument()
    expect(screen.getByText('Last name is required.')).toBeInTheDocument()
    expect(screen.getByText('Accept the terms to continue.')).toBeInTheDocument()
    expect(createAccountApi).not.toHaveBeenCalled()
  })

  it('switches between auth modes', async () => {
    const { props, user } = renderAuth()

    await user.click(screen.getByRole('button', { name: 'Create one' }))

    expect(props.onModeChange).toHaveBeenCalledWith('signup')
  })
})
