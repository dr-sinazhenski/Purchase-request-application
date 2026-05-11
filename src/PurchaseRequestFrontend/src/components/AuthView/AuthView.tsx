import { Eye, PackagePlus } from 'lucide-react'
import type { FormEvent } from 'react'
import { useState } from 'react'

import './AuthView.css'

type AuthMode = 'signin' | 'signup'

type AuthViewProps = {
  mode: AuthMode
  onModeChange: (mode: AuthMode) => void
  onSuccess: () => void
}

type AuthErrors = Partial<Record<string, string>>

export function AuthView({ mode, onModeChange, onSuccess }: AuthViewProps) {
  const [firstName, setFirstName] = useState('Sarah')
  const [lastName, setLastName] = useState('Chen')
  const [email, setEmail] = useState('sarah.chen@acme.com')
  const [password, setPassword] = useState('password123')
  const [confirmPassword, setConfirmPassword] = useState('password123')
  const [rememberMe, setRememberMe] = useState(true)
  const [acceptTerms, setAcceptTerms] = useState(false)
  const [errors, setErrors] = useState<AuthErrors>({})

  const isSignup = mode === 'signup'

  function validate() {
    const nextErrors: AuthErrors = {}

    if (!email.trim()) {
      nextErrors.email = 'Email is required.'
    } else if (!email.includes('@')) {
      nextErrors.email = 'Enter a valid email address.'
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
    }

    setErrors(nextErrors)

    return Object.keys(nextErrors).length === 0
  }

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (validate()) {
      onSuccess()
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
              type="email"
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

          <button className="auth-submit" type="submit">
            {isSignup ? 'Create account' : 'Sign in'}
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
