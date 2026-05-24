import { Eye, PackagePlus } from 'lucide-react'
import type { FormEvent } from 'react'
import { useEffect, useState } from 'react'

import {
  createAccountApi,
  loadAccounts,
  loadRegions,
  loadRoles,
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
  onSuccess: (account: AccountOption) => void
}

type AuthErrors = Partial<Record<string, string>>

export function AuthView({ mode, onModeChange, onSuccess }: AuthViewProps) {
  const [firstName, setFirstName] = useState('Sarah')
  const [lastName, setLastName] = useState('Chen')
  const [email, setEmail] = useState('admin')
  const [password, setPassword] = useState('password123')
  const [confirmPassword, setConfirmPassword] = useState('password123')
  const [rememberMe, setRememberMe] = useState(true)
  const [acceptTerms, setAcceptTerms] = useState(false)
  const [errors, setErrors] = useState<AuthErrors>({})
  const [regions, setRegions] = useState<RegionOption[]>([])
  const [selectedRoleId, setSelectedRoleId] = useState('')
  const [selectedRegionId, setSelectedRegionId] = useState('')
  const [apiError, setApiError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  const isSignup = mode === 'signup'

  useEffect(() => {
    let mounted = true

    async function loadAuthOptions() {
      try {
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
          setApiError('Unable to load account options from API.')
        }
      }
    }

    loadAuthOptions()

    return () => {
      mounted = false
    }
  }, [])

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

      const result = await loadAccounts()

      if (!result.isSuccess || !result.data) {
        setApiError('Unable to load accounts from API.')
        return
      }

      const account = result.data.find(
        (option) => option.login.toLowerCase() === email.trim().toLowerCase(),
      )

      if (!account) {
        setApiError('Account was not found. Create it first or use seeded login.')
        return
      }

      onSuccess(account)
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
                  value={lastName}
                />
                {errors.lastName && <small>{errors.lastName}</small>}
              </label>
            </div>
          )}

          <label className="auth-field">
            <span>
              {isSignup ? 'Work email' : 'Email address'}{' '}
              {isSignup && <strong>*</strong>}
            </span>
            <input
              className={errors.email ? 'auth-input error' : 'auth-input'}
              onChange={(event) => setEmail(event.target.value)}
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
                  type="password"
                  value={password}
                />
                {!isSignup && <Eye size={16} />}
              </span>
              {errors.password && <small>{errors.password}</small>}
            </label>

            {isSignup && (
              <label className="auth-field">
                <span>
                  Confirm password <strong>*</strong>
                </span>
                <input
                  className={
                    errors.confirmPassword ? 'auth-input error' : 'auth-input'
                  }
                  onChange={(event) => setConfirmPassword(event.target.value)}
                  type="password"
                  value={confirmPassword}
                />
                {errors.confirmPassword && (
                  <small>{errors.confirmPassword}</small>
                )}
              </label>
            )}
          </div>

          {isSignup && (
            <>
              <label className="auth-field">
                <span>
                  Region <strong>*</strong>
                </span>
                <select
                  className={errors.region ? 'auth-input error' : 'auth-input'}
                  onChange={(event) => setSelectedRegionId(event.target.value)}
                  value={selectedRegionId}
                >
                  {regions.map((region) => (
                    <option key={region.id} value={region.id}>
                      {region.name} ({region.currency})
                    </option>
                  ))}
                </select>
                {errors.region && <small>{errors.region}</small>}
                <small>New accounts are created as Requester.</small>
              </label>
            </>
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
              <label className="auth-check-row">
                <input
                  checked={acceptTerms}
                  onChange={(event) => setAcceptTerms(event.target.checked)}
                  type="checkbox"
                />
                I agree to the Terms and Privacy Policy
              </label>
              {errors.terms && <small className="auth-terms-error">{errors.terms}</small>}
            </>
          )}

          {apiError && <small className="auth-api-error">{apiError}</small>}

          {!isSignup && (
            <p className="auth-note">
              The backend stores accounts but does not verify passwords yet.
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
