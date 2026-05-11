import { LogOut, User } from 'lucide-react'

import './ProfileView.css'

type ProfileViewProps = {
  onLogout: () => void
}

export function ProfileView({ onLogout }: ProfileViewProps) {
  return (
    <section className="profile-page">
      <header className="profile-header">
        <div>
          <h1>Profile & Settings</h1>
          <p>Manage your account and preferences</p>
        </div>
      </header>

      <div className="profile-layout">
        <aside className="profile-nav-card">
          <button className="profile-nav-item active" type="button">
            <User size={15} />
            Profile
          </button>
          <button
            className="profile-nav-item danger"
            onClick={onLogout}
            type="button"
          >
            <LogOut size={15} />
            Log out
          </button>
        </aside>

        <div className="profile-content">
          <div className="profile-card">
            <div className="avatar-upload">
              <div className="profile-avatar-lg">SC</div>
              <div>
                <strong>Sarah Chen</strong>
                <p>Upload a photo - PNG or JPG, max 2MB</p>
                <div className="profile-actions">
                  <button className="btn compact" type="button">
                    Upload photo
                  </button>
                  <button className="btn compact" type="button">
                    Remove
                  </button>
                </div>
              </div>
            </div>

            <div className="profile-form-row">
              <label className="profile-field">
                <span>First name</span>
                <input defaultValue="Sarah" />
              </label>
              <label className="profile-field">
                <span>Last name</span>
                <input defaultValue="Chen" />
              </label>
            </div>

            <label className="profile-field">
              <span>Work email</span>
              <input defaultValue="sarah.chen@acme.com" type="email" />
            </label>

            <label className="profile-field">
              <span>Role</span>
              <input defaultValue="Approver" disabled />
              <small>Roles are managed by your admin</small>
            </label>

            <div className="profile-save-row">
              <button className="btn compact" type="button">
                Cancel
              </button>
              <button className="btn primary compact" type="button">
                Save changes
              </button>
            </div>
          </div>

        </div>
      </div>
    </section>
  )
}
