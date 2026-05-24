import { AlertTriangle, ArrowLeft, Search, Shield, UserRound } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'

import {
  loadAccounts,
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
import { Field } from '../Field/Field'
import { Topbar } from '../Topbar/Topbar'
import './AdminView.css'

type AdminViewProps = {
  onBackToUsers?: () => void
  onOpenUser: (userId: string) => void
  selectedUserId?: string
}

const editableRoleNames = ['Requester', 'Approver', 'Admin']

function getInitials(name = '') {
  return (
    name
      .split(' ')
      .filter(Boolean)
      .map((part) => part[0])
      .join('')
      .slice(0, 2)
      .toUpperCase() || 'U'
  )
}

export function AdminView({
  onBackToUsers,
  onOpenUser,
  selectedUserId,
}: AdminViewProps) {
  const [accounts, setAccounts] = useState<AccountOption[]>([])
  const [roles, setRoles] = useState<RoleOption[]>([])
  const [regions, setRegions] = useState<RegionOption[]>([])
  const [approverProfiles, setApproverProfiles] = useState<
    ApproverProfileOption[]
  >([])
  const [searchQuery, setSearchQuery] = useState('')
  const [selectedRoleId, setSelectedRoleId] = useState('')
  const [selectedRegionId, setSelectedRegionId] = useState('')
  const [selectedApproverProfileId, setSelectedApproverProfileId] = useState('')
  const [password, setPassword] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')

  const selectedAccount = accounts.find((account) => account.id === selectedUserId)
  const editableRoles = roles.filter((role) =>
    editableRoleNames.includes(role.name),
  )
  const selectedRole = editableRoles.find((role) => role.id === selectedRoleId)
  const shouldShowApproverProfile = selectedRole?.name === 'Approver'

  const filteredAccounts = useMemo(() => {
    const normalizedSearch = searchQuery.trim().toLowerCase()

    if (!normalizedSearch) {
      return accounts
    }

    return accounts.filter((account) =>
      [
        account.name,
        account.login,
        account.regionName,
        account.approverProfileName ?? '',
        ...account.roleNames,
      ]
        .join(' ')
        .toLowerCase()
        .includes(normalizedSearch),
    )
  }, [accounts, searchQuery])

  useEffect(() => {
    let mounted = true

    async function loadAdminData() {
      try {
        setIsLoading(true)
        setError('')
        const [accountsResult, rolesResult, regionsResult, profilesResult] =
          await Promise.all([
            loadAccounts(),
            loadRoles(),
            loadRegions(),
            loadApproverProfiles(),
          ])

        if (!mounted) return

        if (accountsResult.isSuccess && accountsResult.data) {
          setAccounts(accountsResult.data)
        }

        if (rolesResult.isSuccess && rolesResult.data) {
          setRoles(rolesResult.data)
        }

        if (regionsResult.isSuccess && regionsResult.data) {
          setRegions(regionsResult.data)
        }

        if (profilesResult.isSuccess && profilesResult.data) {
          setApproverProfiles(profilesResult.data)
        }
      } catch (loadError) {
        if (mounted) {
          setError(
            loadError instanceof Error
              ? loadError.message
              : 'Unable to load users.',
          )
        }
      } finally {
        if (mounted) {
          setIsLoading(false)
        }
      }
    }

    loadAdminData()

    return () => {
      mounted = false
    }
  }, [])

  useEffect(() => {
    if (!selectedAccount) {
      setSelectedRoleId('')
      setSelectedRegionId('')
      setSelectedApproverProfileId('')
      setPassword('')
      return
    }

    const editableRole =
      editableRoles.find((role) => selectedAccount.roleIds.includes(role.id)) ??
      editableRoles.find((role) => role.name === 'Requester')

    setSelectedRoleId(editableRole?.id ?? '')
    setSelectedRegionId(selectedAccount.regionId)
    setSelectedApproverProfileId(selectedAccount.approverProfileId ?? '')
    setPassword('')
    setMessage('')
    setError('')
  }, [editableRoles, selectedAccount])

  async function saveUserRole() {
    if (!selectedAccount) {
      setError('Select a user before saving.')
      return
    }

    if (!selectedRoleId) {
      setError('Choose a role for this user.')
      return
    }

    if (!selectedRegionId) {
      setError('Choose a region for this user.')
      return
    }

    if (!password) {
      setError('Password is required because the backend account update needs it.')
      return
    }

    try {
      setIsSaving(true)
      setMessage('')
      setError('')
      const result = await updateAccountApi({
        id: selectedAccount.id,
        login: selectedAccount.login,
        password,
        name: selectedAccount.name,
        regionId: selectedRegionId,
        approverProfileId: shouldShowApproverProfile
          ? selectedApproverProfileId || null
          : null,
        roleIds: [selectedRoleId],
      })

      if (!result.isSuccess || !result.data) {
        setError(result.error?.message ?? 'Unable to update user role.')
        return
      }

      setAccounts((currentAccounts) =>
        currentAccounts.map((account) =>
          account.id === result.data?.id ? result.data : account,
        ),
      )
      setPassword('')
      setMessage('User role updated.')
    } catch (saveError) {
      setError(
        saveError instanceof Error ? saveError.message : 'User update failed.',
      )
    } finally {
      setIsSaving(false)
    }
  }

  if (selectedUserId) {
    return (
      <>
        <Topbar title="User profile" />
        <section className="content-area admin-layout">
          <button
            className="admin-back-button"
            onClick={onBackToUsers}
            type="button"
          >
            <ArrowLeft size={16} />
            Users
          </button>

          {!selectedAccount && !isLoading && (
            <div className="notice danger">
              <AlertTriangle size={18} />
              <div>
                <strong>User not found</strong>
                <span>This account is not available in the backend response.</span>
              </div>
            </div>
          )}

          {selectedAccount && (
            <div className="admin-user-layout">
              <div className="panel admin-user-card">
                <div className="admin-user-hero">
                  <div className="admin-avatar">
                    {getInitials(selectedAccount.name)}
                  </div>
                  <div>
                    <h2>{selectedAccount.name}</h2>
                    <p>{selectedAccount.login}</p>
                  </div>
                </div>

                <div className="admin-meta-list">
                  <span>
                    <strong>Current role</strong>
                    {selectedAccount.roleNames.join(', ') || 'No role'}
                  </span>
                  <span>
                    <strong>Region</strong>
                    {selectedAccount.regionName}
                  </span>
                  <span>
                    <strong>Approver profile</strong>
                    {selectedAccount.approverProfileName ?? 'None'}
                  </span>
                </div>
              </div>

              <div className="panel admin-edit-card">
                <h2>Change access</h2>
                <Field label="Role">
                  <select
                    onChange={(event) => setSelectedRoleId(event.target.value)}
                    value={selectedRoleId}
                  >
                    {editableRoles.map((role) => (
                      <option key={role.id} value={role.id}>
                        {role.name}
                      </option>
                    ))}
                  </select>
                </Field>

                <Field label="Region">
                  <select
                    onChange={(event) => setSelectedRegionId(event.target.value)}
                    value={selectedRegionId}
                  >
                    {regions.map((region) => (
                      <option key={region.id} value={region.id}>
                        {region.name} ({region.currency})
                      </option>
                    ))}
                  </select>
                </Field>

                {shouldShowApproverProfile && (
                  <Field label="Approver profile">
                    <select
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
                  </Field>
                )}

                <Field label="Password">
                  <input
                    onChange={(event) => setPassword(event.target.value)}
                    placeholder="Required by backend account update"
                    type="password"
                    value={password}
                  />
                </Field>

                {(error || message) && (
                  <div className={error ? 'notice danger' : 'notice success'}>
                    {error ? <AlertTriangle size={18} /> : <Shield size={18} />}
                    <div>
                      <strong>{error ? 'Update failed' : 'Update saved'}</strong>
                      <span>{error || message}</span>
                    </div>
                  </div>
                )}

                <div className="form-actions">
                  <button
                    className="btn"
                    onClick={onBackToUsers}
                    type="button"
                  >
                    Cancel
                  </button>
                  <button
                    className="btn primary"
                    disabled={isSaving}
                    onClick={saveUserRole}
                    type="button"
                  >
                    {isSaving ? 'Saving...' : 'Save user'}
                  </button>
                </div>
              </div>
            </div>
          )}
        </section>
      </>
    )
  }

  return (
    <>
      <Topbar title="Admin" />
      <section className="content-area admin-layout">
        <div className="panel admin-users-panel">
          <div className="admin-users-header">
            <div>
              <p className="eyebrow">User management</p>
              <h2>Users</h2>
            </div>
            <label className="admin-search">
              <Search size={16} />
              <input
                onChange={(event) => setSearchQuery(event.target.value)}
                placeholder="Search users"
                value={searchQuery}
              />
            </label>
          </div>

          {error && (
            <div className="notice danger">
              <AlertTriangle size={18} />
              <div>
                <strong>Users failed to load</strong>
                <span>{error}</span>
              </div>
            </div>
          )}

          {isLoading ? (
            <p className="admin-empty">Loading users...</p>
          ) : (
            <div className="admin-user-list">
              {filteredAccounts.map((account) => (
                <button
                  className="admin-user-row"
                  key={account.id}
                  onClick={() => onOpenUser(account.id)}
                  type="button"
                >
                  <span className="admin-avatar small">
                    {getInitials(account.name)}
                  </span>
                  <span>
                    <strong>{account.name}</strong>
                    <small>{account.login}</small>
                  </span>
                  <span>{account.roleNames.join(', ') || 'No role'}</span>
                  <span>{account.regionName}</span>
                  <UserRound size={18} />
                </button>
              ))}
              {filteredAccounts.length === 0 && (
                <p className="admin-empty">No users match your search.</p>
              )}
            </div>
          )}
        </div>
      </section>
    </>
  )
}
