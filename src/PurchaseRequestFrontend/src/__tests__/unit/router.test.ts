import { getRouteScreen, parseRoute, routeToPath } from '../../router'

describe('router', () => {
  it.each([
    ['/', { screen: 'requests' }],
    ['/requests', { screen: 'requests' }],
    ['/requests/new', { screen: 'create' }],
    ['/approval', { screen: 'approval' }],
    ['/admin', { screen: 'admin' }],
    ['/profile', { screen: 'profile' }],
    ['/privacy-policy', { screen: 'privacy' }],
    ['/sign-in', { screen: 'signin' }],
    ['/sign-up', { screen: 'signup' }],
  ] as const)('parses %s', (path, expectedRoute) => {
    expect(parseRoute(path)).toEqual(expectedRoute)
  })

  it('parses detail, edit, approval, and admin user routes with ids', () => {
    expect(parseRoute('/requests/abc')).toEqual({
      screen: 'detail',
      requestId: 'abc',
    })
    expect(parseRoute('/requests/abc/edit')).toEqual({
      screen: 'edit',
      requestId: 'abc',
    })
    expect(parseRoute('/requests/abc/approval')).toEqual({
      screen: 'approval',
      requestId: 'abc',
    })
    expect(parseRoute('/admin/users/user-1')).toEqual({
      screen: 'adminUser',
      userId: 'user-1',
    })
  })

  it('falls back to requests for unknown paths', () => {
    expect(parseRoute('/unknown')).toEqual({ screen: 'requests' })
  })

  it.each([
    [{ screen: 'requests' }, '/requests'],
    [{ screen: 'create' }, '/requests/new'],
    [{ screen: 'detail', requestId: 'abc' }, '/requests/abc'],
    [{ screen: 'edit', requestId: 'abc' }, '/requests/abc/edit'],
    [{ screen: 'approval' }, '/approval'],
    [{ screen: 'approval', requestId: 'abc' }, '/requests/abc/approval'],
    [{ screen: 'admin' }, '/admin'],
    [{ screen: 'adminUser', userId: 'user-1' }, '/admin/users/user-1'],
    [{ screen: 'profile' }, '/profile'],
    [{ screen: 'privacy' }, '/privacy-policy'],
    [{ screen: 'signin' }, '/sign-in'],
    [{ screen: 'signup' }, '/sign-up'],
  ] as const)('builds a path for %#', (route, expectedPath) => {
    expect(routeToPath(route)).toBe(expectedPath)
  })

  it('returns the screen from a route', () => {
    expect(getRouteScreen({ screen: 'privacy' })).toBe('privacy')
  })
})
