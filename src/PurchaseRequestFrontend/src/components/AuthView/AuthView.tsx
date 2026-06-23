import { Eye, EyeOff, PackagePlus } from 'lucide-react'
import type { FormEvent } from 'react'
import { useEffect, useState } from 'react'

import {
  createAccountApi,
  loginAccountApi,
  loadAccounts,
  loadRegions,
  loadRoles,
  setAuthToken,
} from '../../api'
import type {
  AccountOption,
  RegionOption,
} from '../../api'
import './AuthView.css'

type AuthMode = 'signin' | 'signup'

type AuthViewProps = {
  mode: AuthMode
  onModeChange: (mode: AuthMode) => void
  onPrivacyPolicy: () => void
  onSuccess: (account: AccountOption) => void
}

type AuthErrors = Partial<Record<string, string>>

function decodeJwtPayload(token: string) {
  try {
    const payload = token.split('.')[1]
    const normalizedPayload = payload.replace(/-/g, '+').replace(/_/g, '/')
    const paddedPayload = normalizedPayload.padEnd(
      normalizedPayload.length + ((4 - (normalizedPayload.length % 4)) % 4),
      '=',
    )
    const json = window.atob(paddedPayload)

    return JSON.parse(json) as Record<string, string | string[]>
  } catch {
    return {}
  }
}

function getClaim(payload: Record<string, string | string[]>, claim: string) {
  const value = payload[claim]

  return Array.isArray(value) ? value[0] : value
}

export function AuthView({
  mode,
  onModeChange,
  onPrivacyPolicy,
  onSuccess,
}: AuthViewProps) {
  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [showPassword, setShowPassword] = useState(false)
  const [showConfirmPassword, setShowConfirmPassword] = useState(false)
  const [rememberMe, setRememberMe] = useState(true)
  const [acceptTerms, setAcceptTerms] = useState(false)
  const [errors, setErrors] = useState<AuthErrors>({})
  const [regions, setRegions] = useState<RegionOption[]>([])
  const [selectedRoleId, setSelectedRoleId] = useState('')
  const [selectedRegionId, setSelectedRegionId] = useState('')
  const [apiError, setApiError] = useState('')
  const [isLoadingAuthOptions, setIsLoadingAuthOptions] = useState(false)
  const [isSubmitting, setIsSubmitting] = useState(false)

  const isSignup = mode === 'signup'

  useEffect(() => {
    let mounted = true

    async function loadAuthOptions() {
      if (!isSignup) {
        return
      }

      try {
        setIsLoadingAuthOptions(true)
        setApiError('')
        const [rolesResult, regionsResult] = await Promise.all([
          loadRoles(),
          loadRegions(),
        ])

        if (!mounted) return

        if (rolesResult.isSuccess && rolesResult.data) {
          const requesterRole =
            rolesResult.data.find((role) => role.name === 'Requester') ??
            rolesResult.data[0]
          setSelectedRoleId(requesterRole?.id ?? '')
        }

        if (regionsResult.isSuccess && regionsResult.data) {
          setRegions(regionsResult.data)
          const europeRegion =
            regionsResult.data.find((region) => region.name === 'Europe') ??
            regionsResult.data[0]
          setSelectedRegionId(europeRegion?.id ?? '')
        }
      } catch (error) {
        if (mounted) {
          setRegions([])
          setSelectedRegionId('')
          setSelectedRoleId('')
          setApiError(
            'Unable to load signup options from API.',
          )
        }
      } finally {
        if (mounted) {
          setIsLoadingAuthOptions(false)
        }
      }
    }

    loadAuthOptions()

    return () => {
      mounted = false
    }
  }, [isSignup])

  function validate() {
    const nextErrors: AuthErrors = {}

    if (!email.trim()) {
      nextErrors.email = 'Email is required.'
    }

    if (!password) {
      nextErrors.password = 'Password is required.'
    }

    if (isSignup) {
      if (!firstName.trim()) {
        nextErrors.firstName = 'First name is required.'
      }

      if (!lastName.trim()) {
        nextErrors.lastName = 'Last name is required.'
      }

      if (password !== confirmPassword) {
        nextErrors.confirmPassword = 'Passwords do not match.'
      }

      if (!acceptTerms) {
        nextErrors.terms = 'Accept the terms to continue.'
      }

      if (!selectedRegionId) {
        nextErrors.region = 'Region is required.'
      }

      if (!selectedRoleId) {
        nextErrors.role = 'Role is required.'
      }
    }

    setErrors(nextErrors)

    return Object.keys(nextErrors).length === 0
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!validate()) {
      return
    }

    try {
      setApiError('')
      setIsSubmitting(true)

      if (isSignup) {
        if (!selectedRegionId || !selectedRoleId) {
          setApiError(
            'Account creation needs region and requester role data from the backend.',
          )
          return
        }

        const result = await createAccountApi({
          login: email.trim(),
          password,
          name: `${firstName.trim()} ${lastName.trim()}`.trim(),
          regionId: selectedRegionId,
          approverProfileId: null,
          roleIds: [selectedRoleId],
        })

        if (!result.isSuccess || !result.data) {
          setApiError('Unable to create account.')
          return
        }

        onSuccess(result.data)
        return
      }

      const result = await loginAccountApi({
        login: email.trim(),
        password,
      })

      if (!result.isSuccess || !result.data) {
        setApiError('Invalid login or password.')
        return
      }

      const { token } = result.data
      const tokenPayload = decodeJwtPayload(token)
      const accountId =
        getClaim(
          tokenPayload,
          'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier',
        ) ?? ''

      setAuthToken(token)

      const signedInAccount: AccountOption = {
        id: accountId,
        login: email.trim(),
        name: result.data.name,
        regionId: '',
        regionName: '',
        approverProfileId: undefined,
        approverProfileName: undefined,
        roleIds: [],
        roleNames: result.data.roles,
        token,
      }

      if (result.data.roles.includes('Admin')) {
        try {
          const accountsResult = await loadAccounts()
          const fullAccount = accountsResult.data?.find(
            (option) => option.id === accountId,
          )

          onSuccess(fullAccount ? { ...fullAccount, token } : signedInAccount)
          return
        } catch {
          onSuccess(signedInAccount)
          return
        }
      }

      onSuccess(signedInAccount)
    } catch (error) {
      setApiError(
        error instanceof Error ? error.message : 'Account request failed.',
      )
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <main className="auth-page">
      <section className={isSignup ? 'auth-card auth-card-wide' : 'auth-card'}>
        <div className="auth-logo">
          <div className="auth-logo-icon">
            <PackagePlus size={18} strokeWidth={2.4} />
          </div>
          <span>ProcureFlow</span>
        </div>

        <h1>{isSignup ? 'Create your account' : 'Welcome back'}</h1>
        <p className="auth-subtitle">
          {isSignup
            ? 'Start managing procurement in minutes'
            : 'Sign in to manage purchase requests'}
        </p>

        <form onSubmit={handleSubmit}>
          {isSignup && (
            <div className="auth-form-row">
              <label className="auth-field">
                <span>
                  First name <strong>*</strong>
                </span>
                <input
                  className={errors.firstName ? 'auth-input error' : 'auth-input'}
                  onChange={(event) => setFirstName(event.target.value)}
                  placeholder="First name"
                  value={firstName}
                />
                {errors.firstName && <small>{errors.firstName}</small>}
              </label>

              <label className="auth-field">
                <span>
                  Last name <strong>*</strong>
                </span>
                <input
                  className={errors.lastName ? 'auth-input error' : 'auth-input'}
                  onChange={(event) => setLastName(event.target.value)}
                  placeholder="Last name"
                  value={lastName}
                />
                {errors.lastName && <small>{errors.lastName}</small>}
              </label>
            </div>
          )}

          <label className="auth-field">
            <span>
              {isSignup ? 'Work email' : 'Login'}{' '}
              {isSignup && <strong>*</strong>}
            </span>
            <input
              className={errors.email ? 'auth-input error' : 'auth-input'}
              onChange={(event) => setEmail(event.target.value)}
              placeholder={isSignup ? 'Work email' : 'Login'}
              value={email}
            />
            {errors.email && <small>{errors.email}</small>}
          </label>

          <div className={isSignup ? 'auth-form-row' : undefined}>
            <label className="auth-field">
              <span>
                Password {isSignup && <strong>*</strong>}
              </span>
              <span className="auth-input-wrap">
                <input
                  className={errors.password ? 'auth-input error' : 'auth-input'}
                  onChange={(event) => setPassword(event.target.value)}
                  placeholder="Password"
                  type={showPassword ? 'text' : 'password'}
                  value={password}
                />
                <button
                  aria-label={showPassword ? 'Hide password' : 'Show password'}
                  className="auth-password-toggle"
                  onClick={() => setShowPassword((isVisible) => !isVisible)}
                  title={showPassword ? 'Hide password' : 'Show password'}
                  type="button"
                >
                  {showPassword ? <EyeOff size={16} /> : <Eye size={16} />}
                </button>
              </span>
              {errors.password && <small>{errors.password}</small>}
            </label>

            {isSignup && (
              <label className="auth-field">
                <span>
                  Confirm password <strong>*</strong>
                </span>
                <span className="auth-input-wrap">
                  <input
                    className={
                      errors.confirmPassword ? 'auth-input error' : 'auth-input'
                    }
                    onChange={(event) => setConfirmPassword(event.target.value)}
                    placeholder="Confirm password"
                    type={showConfirmPassword ? 'text' : 'password'}
                    value={confirmPassword}
                  />
                  <button
                    aria-label={
                      showConfirmPassword
                        ? 'Hide confirm password'
                        : 'Show confirm password'
                    }
                    className="auth-password-toggle"
                    onClick={() =>
                      setShowConfirmPassword((isVisible) => !isVisible)
                    }
                    title={
                      showConfirmPassword
                        ? 'Hide confirm password'
                        : 'Show confirm password'
                    }
                    type="button"
                  >
                    {showConfirmPassword ? (
                      <EyeOff size={16} />
                    ) : (
                      <Eye size={16} />
                    )}
                  </button>
                </span>
                {errors.confirmPassword && (
                  <small>{errors.confirmPassword}</small>
                )}
              </label>
            )}
          </div>

          {isSignup && (
            <label className="auth-field">
              <span>
                Region <strong>*</strong>
              </span>
              <select
                className={errors.region ? 'auth-input error' : 'auth-input'}
                disabled={isLoadingAuthOptions || regions.length === 0}
                onChange={(event) => setSelectedRegionId(event.target.value)}
                value={selectedRegionId}
              >
                {isLoadingAuthOptions && (
                  <option value="">Loading regions from API...</option>
                )}
                {!isLoadingAuthOptions && regions.length === 0 && (
                  <option value="">No regions returned from API</option>
                )}
                {regions.map((region) => (
                  <option key={region.id} value={region.id}>
                    {region.name} ({region.currency})
                  </option>
                ))}
              </select>
              {errors.region && <small>{errors.region}</small>}
              <small>New accounts are created as Requester.</small>
            </label>
          )}

          {!isSignup ? (
            <div className="auth-check-row auth-split-row">
              <label>
                <input
                  checked={rememberMe}
                  onChange={(event) => setRememberMe(event.target.checked)}
                  type="checkbox"
                />
                Remember me
              </label>
              <button className="auth-link" type="button">
                Forgot password?
              </button>
            </div>
          ) : (
            <>
              <div className="auth-check-row">
                <label>
                  <input
                    checked={acceptTerms}
                    onChange={(event) => setAcceptTerms(event.target.checked)}
                    type="checkbox"
                  />
                  I agree to the Terms and
                </label>
                <button
                  className="auth-link"
                  onClick={onPrivacyPolicy}
                  type="button"
                >
                  Privacy Policy
                </button>
              </div>
              {errors.terms && <small className="auth-terms-error">{errors.terms}</small>}
            </>
          )}

          {apiError && <small className="auth-api-error">{apiError}</small>}

          {!isSignup && (
            <p className="auth-note">
              Seeded accounts use logins like admin and passwords like
              hashed_password_4.
            </p>
          )}

          <button className="auth-submit" disabled={isSubmitting} type="submit">
            {isSubmitting
              ? isSignup
                ? 'Creating...'
                : 'Signing in...'
              : isSignup
                ? 'Create account'
                : 'Sign in'}
          </button>
        </form>

        <p className="auth-footer">
          {isSignup ? 'Already have an account?' : "Don't have an account?"}{' '}
          <button
            className="auth-link"
            onClick={() => onModeChange(isSignup ? 'signin' : 'signup')}
            type="button"
          >
            {isSignup ? 'Sign in' : 'Create one'}
          </button>
        </p>
      </section>
    </main>
  )
}
