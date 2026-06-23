module.exports = {
  plugins: [
    function transformViteImportMetaEnv({ types: t }) {
      return {
        name: 'transform-vite-import-meta-env-for-jest',
        visitor: {
          MemberExpression(path) {
            const { node } = path
            const envExpression = node.object

            if (
              !t.isIdentifier(node.property) ||
              !t.isMemberExpression(envExpression) ||
              !t.isIdentifier(envExpression.property, { name: 'env' }) ||
              !t.isMetaProperty(envExpression.object) ||
              envExpression.object.meta.name !== 'import' ||
              envExpression.object.property.name !== 'meta'
            ) {
              return
            }

            path.replaceWith(
              t.memberExpression(
                t.memberExpression(t.identifier('process'), t.identifier('env')),
                t.identifier(node.property.name),
              ),
            )
          },
        },
      }
    },
  ],
  presets: [
    ['@babel/preset-env', { targets: { node: 'current' } }],
    ['@babel/preset-react', { runtime: 'automatic' }],
    '@babel/preset-typescript',
  ],
}
