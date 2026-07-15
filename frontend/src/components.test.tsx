import { describe, expect, it } from 'vitest'
import { formatCurrency } from './components'

describe('formatCurrency', () => {
  it('formats monetary values in Brazilian reais', () => {
    const formatted = formatCurrency(1234.56)

    expect(formatted).toContain('R$')
    expect(formatted).toContain('1.234,56')
  })
})
