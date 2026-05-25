export function hasRole(
  account: { roleNames: string[] } | undefined,
  roleName: string,
) {
  return Boolean(
    account?.roleNames.some(
      (name) => name.toLowerCase() === roleName.toLowerCase(),
    ),
  )
}

export function getVisibleRoleNames(roleNames: string[] = []) {
  const hasApprover = roleNames.some(
    (roleName) => roleName.toLowerCase() === 'approver',
  )
  const hasAdmin = roleNames.some((roleName) => roleName.toLowerCase() === 'admin')

  if (!hasApprover || hasAdmin) {
    return roleNames
  }

  return roleNames.filter(
    (roleName) => roleName.toLowerCase() !== 'requester',
  )
}

export function getPrimaryRoleName(roleNames: string[] = []) {
  const visibleRoles = getVisibleRoleNames(roleNames)

  return visibleRoles[0] ?? 'User'
}
