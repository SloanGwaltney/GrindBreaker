import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import type { Candidacy, CreateCandidacyRequest, UpdateCandidacyRequest, UpdateCandidacyStatusRequest } from '../lib/types'

interface RPCResult<T> {
  errorMessage?: string
  isError: boolean
  data?: T
}

// RPC functions using window object bindings
const fetchCandidacies = async (): Promise<RPCResult<Candidacy[]>> => {
  if (typeof window !== 'undefined' && (window as any).GRIND_BREAKER_GetAllCandidacies) {
    const result = await (window as any).GRIND_BREAKER_GetAllCandidacies()
    console.log('GetAllCandidacies result:', result)
    return result
  }
  throw new Error('RPC function not available')
}

const fetchCandidacy = async (id: string): Promise<RPCResult<Candidacy>> => {
  if (typeof window !== 'undefined' && (window as any).GRIND_BREAKER_GetCandidacy) {
    const result = await (window as any).GRIND_BREAKER_GetCandidacy(id)
    console.log('GetCandidacy result:', result)
    return result
  }
  throw new Error('RPC function not available')
}

const createCandidacy = async (candidacy: CreateCandidacyRequest): Promise<string> => {
  if (typeof window !== 'undefined' && (window as any).GRIND_BREAKER_SaveCandidacy) {
    const result = await (window as any).GRIND_BREAKER_SaveCandidacy(candidacy)
    console.log('SaveCandidacy result:', result)
    if (result.isError) {
      throw new Error(result.errorMessage || 'Failed to create candidacy')
    }
    return result.data || ''
  }
  throw new Error('RPC function not available')
}

const updateCandidacy = async (candidacy: UpdateCandidacyRequest): Promise<string> => {
  console.log('updateCandidacy called with:', candidacy)
  if (typeof window !== 'undefined' && (window as any).GRIND_BREAKER_UpdateCandidacy) {
    try {
      const result = await (window as any).GRIND_BREAKER_UpdateCandidacy(candidacy)
      console.log('UpdateCandidacy result:', result)
      if (result.isError) {
        throw new Error(result.errorMessage || 'Failed to update candidacy')
      }
      return result.data || ''
    } catch (error) {
      console.error('UpdateCandidacy RPC call failed:', error)
      throw error
    }
  }
  throw new Error('RPC function not available')
}

const deleteCandidacy = async (id: string): Promise<string> => {
  if (typeof window !== 'undefined' && (window as any).GRIND_BREAKER_DeleteCandidacy) {
    const result = await (window as any).GRIND_BREAKER_DeleteCandidacy(id)
    console.log('DeleteCandidacy result:', result)
    if (result.isError) {
      throw new Error(result.errorMessage || 'Failed to delete candidacy')
    }
    return result.data || ''
  }
  throw new Error('RPC function not available')
}

const updateCandidacyStatus = async ({ id, status }: UpdateCandidacyStatusRequest): Promise<string> => {
  if (typeof window !== 'undefined' && (window as any).GRIND_BREAKER_UpdateCandidacyStatus) {
    const result = await (window as any).GRIND_BREAKER_UpdateCandidacyStatus(id, status)
    console.log('UpdateCandidacyStatus result:', result)
    if (result.isError) {
      throw new Error(result.errorMessage || 'Failed to update candidacy status')
    }
    return result.data || ''
  }
  throw new Error('RPC function not available')
}

export const useCandidacies = () => {
  return useQuery({
    queryKey: ['candidacies'],
    queryFn: fetchCandidacies,
    retry: false,
  })
}

export const useCandidacy = (id: string) => {
  return useQuery({
    queryKey: ['candidacies', id],
    queryFn: () => fetchCandidacy(id),
    enabled: !!id,
    retry: false,
  })
}

export const useCreateCandidacy = () => {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: createCandidacy,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['candidacies'] })
    },
  })
}

export const useUpdateCandidacy = () => {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: updateCandidacy,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['candidacies'] })
    },
  })
}

export const useDeleteCandidacy = () => {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: deleteCandidacy,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['candidacies'] })
    },
  })
}

export const useUpdateCandidacyStatus = () => {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: updateCandidacyStatus,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['candidacies'] })
    },
  })
}
