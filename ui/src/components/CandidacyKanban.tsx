import { useState } from 'react'
import {
  DndContext,
  DragOverlay,
  PointerSensor,
  useSensor,
  useSensors,
  useDroppable,
} from '@dnd-kit/core'
import type { DragEndEvent, DragStartEvent } from '@dnd-kit/core'
import {
  SortableContext,
  verticalListSortingStrategy,
} from '@dnd-kit/sortable'
import { Eye, EyeOff } from 'lucide-react'
import { Button } from './ui/button'
import { CandidacyCard } from './CandidacyCard'
import { useCandidacies, useUpdateCandidacyStatus, useDeleteCandidacy } from '../hooks/useCandidacies'
import type { Candidacy } from '../lib/types'
import { CandidacyStatus } from '../lib/types'

const COLUMNS = [
  { id: 'ToApply', title: 'To Apply', status: CandidacyStatus.ToApply },
  { id: 'Applied', title: 'Applied', status: CandidacyStatus.Applied },
  { id: 'PreInterview', title: 'Pre-interview', status: CandidacyStatus.PreInterview },
  { id: 'PostInterview', title: 'Post-interview', status: CandidacyStatus.PostInterview },
  { id: 'Offered', title: 'Offered', status: CandidacyStatus.Offered },
  { id: 'Rejected', title: 'Rejected', status: CandidacyStatus.Rejected },
  { id: 'Ghosted', title: 'Ghosted', status: CandidacyStatus.Ghosted },
  { id: 'Withdrawn', title: 'Withdrawn', status: CandidacyStatus.Withdrawn },
]

const VISIBLE_COLUMNS = ['ToApply', 'Applied', 'PreInterview', 'PostInterview', 'Offered']

interface CandidacyKanbanProps {
  onEditCandidacy: (candidacy: Candidacy) => void
}

export function CandidacyKanban({ onEditCandidacy }: CandidacyKanbanProps) {
  const [showHiddenColumns, setShowHiddenColumns] = useState(false)
  const [activeCandidacy, setActiveCandidacy] = useState<Candidacy | null>(null)
  
  const { data: candidaciesResult, isLoading, error } = useCandidacies()
  const candidacies = candidaciesResult?.data || []
  const updateStatusMutation = useUpdateCandidacyStatus()
  const deleteMutation = useDeleteCandidacy()


  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 8,
      },
    })
  )

  const getCandidaciesByStatus = (status: CandidacyStatus) => {
    return candidacies.filter(candidacy => candidacy.status === status)
  }

  const handleDragStart = (event: DragStartEvent) => {
    const { active } = event
    const candidacy = candidacies.find(c => c.id === active.id)
    setActiveCandidacy(candidacy || null)
  }

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event
    setActiveCandidacy(null)

    console.log('Drag end - active:', active.id, 'over:', over?.id)

    if (!over) {
      console.log('No drop target')
      return
    }

    const candidacy = candidacies.find(c => c.id === active.id)
    const targetColumn = COLUMNS.find(col => col.id === over.id)
    
    console.log('Candidacy:', candidacy?.company, 'current status:', candidacy?.status)
    console.log('Target column:', targetColumn?.title, 'target status:', targetColumn?.status)
    
    if (candidacy && targetColumn && candidacy.status !== targetColumn.status) {
      console.log('Updating status from', candidacy.status, 'to', targetColumn.status)
      updateStatusMutation.mutate({
        id: candidacy.id,
        status: targetColumn.status,
      }, {
        onSuccess: () => {
          console.log('Status update successful')
        },
        onError: (error) => {
          console.error('Status update failed:', error)
        }
      })
    } else {
      console.log('No status change needed or missing data')
    }
  }

  const handleDeleteCandidacy = (id: string) => {
    deleteMutation.mutate(id)
  }

  const visibleColumns = showHiddenColumns ? COLUMNS : COLUMNS.filter(col => VISIBLE_COLUMNS.includes(col.id))

  // Droppable column component
  const DroppableColumn = ({ column, children }: { column: typeof COLUMNS[0], children: React.ReactNode }) => {
    const { setNodeRef } = useDroppable({
      id: column.id,
    })

    return (
      <div ref={setNodeRef} className="bg-gray-50 rounded-lg p-4 min-h-[400px]">
        {children}
      </div>
    )
  }

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-lg">Loading candidacies...</div>
      </div>
    )
  }

  if (error) {
    console.error('Error loading candidacies:', error)
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-lg text-red-600">Error loading candidacies: {error.message}</div>
      </div>
    )
  }

  if (candidaciesResult?.isError) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-lg text-red-600">
          Error loading candidacies: {candidaciesResult.errorMessage || 'Unknown error'}
        </div>
      </div>
    )
  }


  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">Job Candidacies</h1>
        <div className="flex gap-2">
          <Button
            variant="outline"
            onClick={() => setShowHiddenColumns(!showHiddenColumns)}
          >
            {showHiddenColumns ? (
              <>
                <EyeOff className="h-4 w-4 mr-2" />
                Hide Inactive
              </>
            ) : (
              <>
                <Eye className="h-4 w-4 mr-2" />
                Show Inactive
              </>
            )}
          </Button>
        </div>
      </div>

      <DndContext
        sensors={sensors}
        onDragStart={handleDragStart}
        onDragEnd={handleDragEnd}
      >
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-5 gap-4">
          {visibleColumns.map((column) => {
            const columnCandidacies = getCandidaciesByStatus(column.status)
            
            return (
              <DroppableColumn key={column.id} column={column}>
                <div className="flex items-center justify-between mb-4">
                  <h3 className="font-semibold text-gray-700">{column.title}</h3>
                  <span className="bg-gray-200 text-gray-600 text-sm px-2 py-1 rounded-full">
                    {columnCandidacies.length}
                  </span>
                </div>

                <SortableContext
                  items={columnCandidacies.map(c => c.id)}
                  strategy={verticalListSortingStrategy}
                >
                  <div className="space-y-3">
                    {columnCandidacies.map((candidacy) => (
                      <CandidacyCard
                        key={candidacy.id}
                        candidacy={candidacy}
                        onEdit={onEditCandidacy}
                        onDelete={handleDeleteCandidacy}
                      />
                    ))}
                  </div>
                </SortableContext>
              </DroppableColumn>
            )
          })}
        </div>

        <DragOverlay>
          {activeCandidacy ? (
            <CandidacyCard
              candidacy={activeCandidacy}
              onEdit={onEditCandidacy}
              onDelete={handleDeleteCandidacy}
            />
          ) : null}
        </DragOverlay>
      </DndContext>
    </div>
  )
}
