import type { Screen } from './types'

export type AppRoute =
  | { screen: 'requests' }
  | { screen: 'create' }
  | { screen: 'detail'; requestId: string }
  | { screen: 'edit'; requestId: string }
  | { screen: 'approval'; requestId?: string }
  | { screen: 'admin' }
  | { screen: 'adminUser'; userId: string }
  | { screen: 'profile' }
  | { screen: 'privacy' }
  | { screen: 'signin' }
  | { screen: 'signup' }

export const appRoutes = {
  approval: '/approval',
  admin: '/admin',
  adminUser: '/admin/users/:id',
  create: '/requests/new',
  detail: '/requests/:id',
  edit: '/requests/:id/edit',
  profile: '/profile',
  privacy: '/privacy-policy',
  requests: '/requests',
  signin: '/sign-in',
  signup: '/sign-up',
} as const

export function parseRoute(pathname: string): AppRoute {
  const segments = pathname.split('/').filter(Boolean)

  if (segments.length === 0) {
    return { screen: 'requests' }
  }

  if (segments[0] === 'approval') {
    return { screen: 'approval' }
  }

  if (segments[0] === 'admin') {
    if (segments[1] === 'users' && segments[2]) {
      return { screen: 'adminUser', userId: segments[2] }
    }

    return { screen: 'admin' }
  }

  if (segments[0] === 'profile') {
    return { screen: 'profile' }
  }

  if (segments[0] === 'privacy-policy') {
    return { screen: 'privacy' }
  }

  if (segments[0] === 'sign-in') {
    return { screen: 'signin' }
  }

  if (segments[0] === 'sign-up') {
    return { screen: 'signup' }
  }

  if (segments[0] !== 'requests') {
    return { screen: 'requests' }
  }

  if (segments.length === 1) {
    return { screen: 'requests' }
  }

  if (segments[1] === 'new') {
    return { screen: 'create' }
  }

  const requestId = segments[1]

  if (segments[2] === 'edit') {
    return { screen: 'edit', requestId }
  }

  if (segments[2] === 'approval') {
    return { screen: 'approval', requestId }
  }

  return { screen: 'detail', requestId }
}

export function routeToPath(route: AppRoute) {
  switch (route.screen) {
    case 'requests':
      return appRoutes.requests
    case 'create':
      return appRoutes.create
    case 'detail':
      return `/requests/${route.requestId}`
    case 'edit':
      return `/requests/${route.requestId}/edit`
    case 'approval':
      return route.requestId
        ? `/requests/${route.requestId}/approval`
        : appRoutes.approval
    case 'admin':
      return appRoutes.admin
    case 'adminUser':
      return `/admin/users/${route.userId}`
    case 'profile':
      return appRoutes.profile
    case 'privacy':
      return appRoutes.privacy
    case 'signin':
      return appRoutes.signin
    case 'signup':
      return appRoutes.signup
  }
}

export function getRouteScreen(route: AppRoute): Screen {
  return route.screen
}
