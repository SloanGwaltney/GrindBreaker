import { createFileRoute } from '@tanstack/react-router'
import { useState } from 'react'
import { Plus } from 'lucide-react'
import { Button } from '../components/ui/button'
import { CandidacyKanban } from '../components/CandidacyKanban'
import { CandidacyForm } from '../components/CandidacyForm'
import { useCreateCandidacy, useUpdateCandidacy } from '../hooks/useCandidacies'
import type { Candidacy, CreateCandidacyRequest, UpdateCandidacyRequest } from '../lib/types'

export const Route = createFileRoute('/candidacies')({
  component: CandidaciesPage,
})

function CandidaciesPage() {
  const [isFormOpen, setIsFormOpen] = useState(false)
  const [editingCandidacy, setEditingCandidacy] = useState<Candidacy | null>(null)
  
  const createMutation = useCreateCandidacy()
  const updateMutation = useUpdateCandidacy()

  const handleCreateCandidacy = () => {
    setEditingCandidacy(null)
    setIsFormOpen(true)
  }

  const handleEditCandidacy = (candidacy: Candidacy) => {
    setEditingCandidacy(candidacy)
    setIsFormOpen(true)
  }

  const handleCloseForm = () => {
    setIsFormOpen(false)
    setEditingCandidacy(null)
  }

  const handleSubmitForm = (data: any) => {
    // Convert Date objects to timestamps for backend
    const convertedData = {
      ...data,
      dateApplied: data.dateApplied.getTime(),
      applicationSteps: data.applicationSteps.map((step: any) => ({
        ...step,
        date: step.date.getTime()
      }))
    }

    // Add id for updates
    if (editingCandidacy) {
      convertedData.id = editingCandidacy.id
    }

    if (editingCandidacy) {
      updateMutation.mutate(convertedData as UpdateCandidacyRequest, {
        onSuccess: () => {
          console.log('Update mutation successful')
          handleCloseForm()
        },
        onError: (error) => {
          console.error('Update mutation failed:', error)
          alert('Failed to update candidacy: ' + error.message)
        },
      })
    } else {
      createMutation.mutate(convertedData as CreateCandidacyRequest, {
        onSuccess: () => {
          console.log('Create mutation successful')
          handleCloseForm()
        },
        onError: (error) => {
          console.error('Create mutation failed:', error)
          alert('Failed to create candidacy: ' + error.message)
        },
      })
    }
  }

  return (
    <div className="min-h-screen bg-gray-100">
      <div className="mb-4 px-6 pt-4">
        <Button onClick={handleCreateCandidacy} className="mb-4">
          <Plus className="h-4 w-4 mr-2" />
          Add New Candidacy
        </Button>
      </div>

      <CandidacyKanban onEditCandidacy={handleEditCandidacy} />

      {isFormOpen && (
        <CandidacyForm
          candidacy={editingCandidacy || undefined}
          onSubmit={handleSubmitForm}
          onCancel={handleCloseForm}
          isLoading={createMutation.isPending || updateMutation.isPending}
        />
      )}
    </div>
  )
}
