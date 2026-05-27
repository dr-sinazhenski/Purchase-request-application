import { LogOut } from 'lucide-react'
import { useEffect, useState } from 'react'

import {
  deleteAccountApi,
  loadApproverProfiles,
  loadRegions,
  loadRoles,
  updateAccountApi,
} from '../../api'
import type {
  AccountOption,
  ApproverProfileOption,
  RegionOption,
  RoleOption,
} from '../../api'
import { getVisibleRoleNames } from '../../utils/roles'
import './ProfileView.css'

type ProfileViewProps = {
  account?: AccountOption
  canManageRoles: boolean
  onAccountChange: (account: AccountOption) => void
  onLogout: () => void
}

function getInitials(name = '') {
  return (
    name
      .split(' ')
      .filter(Boolean)
      .map((part) => part[0])
      .join('')
      .slice(0, 2)
      .toUpperCase() || 'CU'
  )
}

export function ProfileView({
  account,
  canManageRoles,
  onAccountChange,
  onLogout,
}: ProfileViewProps) {
  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')
  const [login, setLogin] = useState('')
  const [password, setPassword] = useState('')
  const [selectedRegionId, setSelectedRegionId] = useState('')
  const [selectedRoleIds, setSelectedRoleIds] = useState<string[]>([])
  const [selectedApproverProfileId, setSelectedApproverProfileId] = useState('')
  const [roles, setRoles] = useState<RoleOption[]>([])
  const [regions, setRegions] = useState<RegionOption[]>([])
  const [approverProfiles, setApproverProfiles] = useState<
    ApproverProfileOption[]
  >([])
  const [isLoadingOptions, setIsLoadingOptions] = useState(false)
  const [isSaving, setIsSaving] = useState(false)
  const [profileMessage, setProfileMessage] = useState('')
  const [profileError, setProfileError] = useState('')

  useEffect(() => {
    const [initialFirstName = '', ...initialLastNameParts] =
      account?.name.split(' ') ?? ['Current', 'User']

    setFirstName(initialFirstName)
    setLastName(initialLastNameParts.join(' '))
    setLogin(account?.login ?? '')
    setSelectedRegionId(account?.regionId ?? '')
    setSelectedRoleIds(account?.roleIds ?? [])
    setSelectedApproverProfileId(account?.approverProfileId ?? '')
  }, [account])

  useEffect(() => {
    let mounted = true

    async function loadProfileOptions() {
      try {
        setIsLoadingOptions(true)
        setProfileError('')
        const [rolesResult, regionsResult, profilesResult] = await Promise.all([
          loadRoles(),
          loadRegions(),
          loadApproverProfiles(),
        ])

        if (!mounted) return

        if (rolesResult.isSuccess && rolesResult.data) {
          setRoles(rolesResult.data)
        }

        if (regionsResult.isSuccess && regionsResult.data) {
          setRegions(regionsResult.data)
        }

        if (profilesResult.isSuccess && profilesResult.data) {
          setApproverProfiles(profilesResult.data)
        }
      } catch (error) {
        if (mounted) {
          setProfileError(
            error instanceof Error
              ? error.message
              : 'Unable to load profile options.',
          )
        }
      } finally {
        if (mounted) {
          setIsLoadingOptions(false)
        }
      }
    }

    loadProfileOptions()

    return () => {
      mounted = false
    }
  }, [])

  const selectedRoleNames = roles
    .filter((role) => selectedRoleIds.includes(role.id))
    .map((role) => role.name)
  const currentRoleNames =
    getVisibleRoleNames(account?.roleNames).join(', ') || 'No role assigned'
  const roleNamesForProfile = canManageRoles
    ? selectedRoleNames
    : getVisibleRoleNames(account?.roleNames)
  const shouldShowApproverProfile = roleNamesForProfile.some((roleName) =>
    ['Approver', 'Admin'].includes(roleName),
  )

  function toggleRole(roleId: string) {
    if (!canManageRoles) {
      return
    }

    setSelectedRoleIds((currentRoleIds) =>
      currentRoleIds.includes(roleId)
        ? currentRoleIds.filter((currentRoleId) => currentRoleId !== roleId)
        : [...currentRoleIds, roleId],
    )
  }

  async function saveProfile() {
    if (!account?.id) {
      setProfileError('Sign in before saving profile changes.')
      return
    }

    if (!login.trim()) {
      setProfileError('Login is required.')
      return
    }

    if (!selectedRegionId) {
      setProfileError('Region is required.')
      return
    }

    if (selectedRoleIds.length === 0) {
      setProfileError('Choose at least one role.')
      return
    }

    if (!password) {
      setProfileError('Password is required because the backend update needs it.')
      return
    }

    try {
      setIsSaving(true)
      setProfileError('')
      setProfileMessage('')
      const result = await updateAccountApi({
        id: account.id,
        login: login.trim(),
        password,
        name: `${firstName.trim()} ${lastName.trim()}`.trim(),
        regionId: selectedRegionId,
        approverProfileId: shouldShowApproverProfile
          ? canManageRoles
            ? selectedApproverProfileId || null
            : account.approverProfileId ?? null
          : null,
        roleIds: canManageRoles ? selectedRoleIds : account.roleIds,
      })

      if (!result.isSuccess || !result.data) {
        setProfileError(result.error?.message ?? 'Unable to update account.')
        return
      }

      onAccountChange(result.data)
      setPassword('')
      setProfileMessage('Profile saved.')
    } catch (error) {
      setProfileError(
        error instanceof Error ? error.message : 'Profile update failed.',
      )
    } finally {
      setIsSaving(false)
    }
  }

  async function deleteProfile() {
    if (!account?.id) {
      setProfileError('Sign in before deleting an account.')
      return
    }

    if (!window.confirm(`Delete account "${account.login}"?`)) {
      return
    }

    try {
      setProfileError('')
      const result = await deleteAccountApi(account.id)

      if (!result.isSuccess) {
        setProfileError(result.error?.message ?? 'Unable to delete account.')
        return
      }

      onLogout()
    } catch (error) {
      setProfileError(
        error instanceof Error ? error.message : 'Account delete failed.',
      )
    }
  }

  return (
    <section className="profile-page">
      <header className="profile-header">
        <div>
          <h1>Profile & Settings</h1>
          <p>Manage your account and preferences</p>
        </div>
        <nav className="profile-topbar-nav" aria-label="Profile actions">
          <button
            className="profile-nav-item danger"
            onClick={onLogout}
            type="button"
          >
            <LogOut size={15} />
            Log out
          </button>
        </nav>
      </header>

      <div className="profile-layout">
        <div className="profile-content">
          <div className="profile-card">
            <div className="avatar-upload">
              <div className="profile-avatar-lg">{getInitials(account?.name)}</div>
              <div>
                <strong>{account?.name ?? 'Current User'}</strong>
                <p>
                  {account
                    ? `${account.login} - ${account.regionName}`
                    : 'Sign in to load backend account data'}
                </p>
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
              <input
                onChange={(event) => setFirstName(event.target.value)}
                value={firstName}
              />
            </label>
            <label className="profile-field">
              <span>Last name</span>
              <input
                onChange={(event) => setLastName(event.target.value)}
                value={lastName}
              />
            </label>
          </div>

          <label className="profile-field">
            <span>Login</span>
              <input
                onChange={(event) => setLogin(event.target.value)}
                value={login}
              />
          </label>

            <label className="profile-field">
              <span>Password</span>
              <input
                onChange={(event) => setPassword(event.target.value)}
                placeholder="Required to save account changes"
                type="password"
                value={password}
              />
            </label>

            <label className="profile-field">
              <span>Region</span>
              <select
                disabled={isLoadingOptions}
                onChange={(event) => setSelectedRegionId(event.target.value)}
                value={selectedRegionId}
              >
                {regions.map((region) => (
                  <option key={region.id} value={region.id}>
                    {region.name} ({region.currency})
                  </option>
                ))}
              </select>
            </label>

            <label className="profile-field">
              <span>Role</span>
              {canManageRoles ? (
                <div className="profile-check-list">
                  {roles.map((role) => (
                    <label key={role.id}>
                      <input
                        checked={selectedRoleIds.includes(role.id)}
                        onChange={() => toggleRole(role.id)}
                        type="checkbox"
                      />
                      {role.name}
                    </label>
                  ))}
                </div>
              ) : (
                <input readOnly value={currentRoleNames} />
              )}
              <small>
                {canManageRoles
                  ? 'Only admins can change account roles.'
                  : 'Only an admin can change your role.'}
              </small>
            </label>

            {shouldShowApproverProfile && canManageRoles && (
              <label className="profile-field">
                <span>Approver profile</span>
                <select
                  disabled={isLoadingOptions}
                  onChange={(event) =>
                    setSelectedApproverProfileId(event.target.value)
                  }
                  value={selectedApproverProfileId}
                >
                  <option value="">None</option>
                  {approverProfiles.map((profile) => (
                    <option key={profile.id} value={profile.id}>
                      {profile.name} ({profile.minAmount}-{profile.maxAmount})
                    </option>
                  ))}
                </select>
              </label>
            )}

            {shouldShowApproverProfile && !canManageRoles && (
              <label className="profile-field">
                <span>Approver profile</span>
                <input readOnly value={account?.approverProfileName ?? 'None'} />
              </label>
            )}

            {(profileError || profileMessage) && (
              <div className={profileError ? 'notice danger' : 'notice success'}>
                <strong>{profileError ? 'Profile error' : 'Profile saved'}</strong>
                <span>{profileError || profileMessage}</span>
              </div>
            )}

            <div className="profile-save-row">
              <button
                className="btn compact danger"
                onClick={deleteProfile}
                type="button"
              >
                Delete account
              </button>
              <button
                className="btn primary compact"
                disabled={isSaving}
                onClick={saveProfile}
                type="button"
              >
                {isSaving ? 'Saving...' : 'Save changes'}
              </button>
            </div>
          </div>

        </div>
      </div>
    </section>
  )
}
