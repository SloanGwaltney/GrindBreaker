import { ReactElement } from 'react'
import { render, RenderOptions } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { vi } from 'vitest'

// Mock the RPC functions
export const mockGetProfile = vi.fn()
export const mockSaveProfile = vi.fn()

// Mock window object with RPC functions
Object.defineProperty(window, 'GRIND_BREAKER_GetProfile', {
  value: mockGetProfile,
  writable: true,
})

Object.defineProperty(window, 'GRIND_BREAKER_SaveProfile', {
  value: mockSaveProfile,
  writable: true,
})

// Create a custom render function that includes providers
const AllTheProviders = ({ children }: { children: React.ReactNode }) => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
      },
      mutations: {
        retry: false,
      },
    },
  })

  return (
    <QueryClientProvider client={queryClient}>
      {children}
    </QueryClientProvider>
  )
}

const customRender = (
  ui: ReactElement,
  options?: Omit<RenderOptions, 'wrapper'>,
) => render(ui, { wrapper: AllTheProviders, ...options })

// Re-export everything
export * from '@testing-library/react'
export { customRender as render }
