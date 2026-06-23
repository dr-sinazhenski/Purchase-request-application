import { PackagePlus } from 'lucide-react'

import './PrivacyPolicyView.css'

type PrivacyPolicyViewProps = {
  onBack: () => void
}

export function PrivacyPolicyView({ onBack }: PrivacyPolicyViewProps) {
  return (
    <main className="privacy-page">
      <section className="privacy-card">
        <div className="privacy-logo">
          <div className="privacy-logo-icon">
            <PackagePlus size={18} strokeWidth={2.4} />
          </div>
          <span>ProcureFlow</span>
        </div>

        <button className="privacy-back" onClick={onBack} type="button">
          ← Back to sign up
        </button>

        <h1>Privacy Policy</h1>
        <p className="privacy-updated">Last updated: June 23, 2026</p>

        <div className="privacy-content">
          <section>
            <h2>Information we collect</h2>
            <p>
              We collect account details such as your name, login, role, region,
              and purchase request activity so ProcureFlow can provide approval
              and procurement workflows.
            </p>
          </section>

          <section>
            <h2>How we use information</h2>
            <p>
              We use this information to authenticate users, route requests to
              approvers, show request history, calculate regional pricing, and
              maintain application security.
            </p>
          </section>

          <section>
            <h2>Data sharing</h2>
            <p>
              Request data may be visible to users involved in the workflow,
              including requesters, approvers, and administrators. We do not sell
              personal data.
            </p>
          </section>

          <section>
            <h2>Retention and security</h2>
            <p>
              We keep account and request data for as long as needed to support
              business records and audit history. Access is controlled through
              user roles and authenticated sessions.
            </p>
          </section>

          <section>
            <h2>Your choices</h2>
            <p>
              You can update account details from your profile. Contact an
              administrator if you need role, region, or account access changes.
            </p>
          </section>
        </div>
      </section>
    </main>
  )
}
