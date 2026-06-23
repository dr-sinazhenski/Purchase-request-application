import { getPrimaryRoleName, getVisibleRoleNames, hasRole } from '../../../utils/roles'

describe('role utilities', () => {
  it('checks roles case-insensitively', () => {
    expect(hasRole({ roleNames: ['Requester'] }, 'requester')).toBe(true)
    expect(hasRole({ roleNames: ['Approver'] }, 'Admin')).toBe(false)
  })

  it('handles missing accounts', () => {
    expect(hasRole(undefined, 'Requester')).toBe(false)
  })

  it('keeps requester visible when approver is absent', () => {
    expect(getVisibleRoleNames(['Requester'])).toEqual(['Requester'])
  })

  it('hides requester when user is also approver', () => {
    expect(getVisibleRoleNames(['Requester', 'Approver'])).toEqual(['Approver'])
  })

  it('keeps all roles visible for admins', () => {
    expect(getVisibleRoleNames(['Requester', 'Approver', 'Admin'])).toEqual([
      'Requester',
      'Approver',
      'Admin',
    ])
  })

  it('returns first visible role as primary role', () => {
    expect(getPrimaryRoleName(['Requester', 'Approver'])).toBe('Approver')
    expect(getPrimaryRoleName(['Admin'])).toBe('Admin')
  })

  it('falls back to User when roles are empty', () => {
    expect(getPrimaryRoleName()).toBe('User')
  })
})
