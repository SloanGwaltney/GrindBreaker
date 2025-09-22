import { useState } from 'react'
import { useSortable } from '@dnd-kit/sortable'
import { CSS } from '@dnd-kit/utilities'
import { Calendar, ExternalLink, Trash2, Edit, ChevronDown, ChevronUp } from 'lucide-react'
import { Button } from './ui/button'
import type { Candidacy } from '../lib/types'

interface CandidacyCardProps {
  candidacy: Candidacy
  onEdit: (candidacy: Candidacy) => void
  onDelete: (id: string) => void
}

export function CandidacyCard({ candidacy, onEdit, onDelete }: CandidacyCardProps) {
  const [isExpanded, setIsExpanded] = useState(false)
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: candidacy.id })

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  }

  const formatDate = (timestamp: number) => {
    return new Date(timestamp).toLocaleDateString()
  }

  const handleDelete = (e: React.MouseEvent) => {
    e.stopPropagation()
    if (window.confirm('Are you sure you want to delete this candidacy?')) {
      onDelete(candidacy.id)
    }
  }

  return (
    <div
      ref={setNodeRef}
      style={style}
      {...attributes}
      {...listeners}
      className={`bg-white rounded-lg shadow-md p-4 cursor-grab active:cursor-grabbing border-l-4 ${
        isDragging ? 'opacity-50' : ''
      } ${
        candidacy.status === 5 ? 'border-l-red-500' : // Rejected
        candidacy.status === 6 ? 'border-l-gray-500' : // Ghosted
        candidacy.status === 7 ? 'border-l-yellow-500' : // Withdrawn
        candidacy.status === 4 ? 'border-l-green-500' : // Offered
        candidacy.status === 3 ? 'border-l-blue-500' : // PostInterview
        candidacy.status === 2 ? 'border-l-purple-500' : // PreInterview
        candidacy.status === 1 ? 'border-l-orange-500' : // Applied
        'border-l-gray-300' // ToApply
      }`}
    >
      <div className="flex justify-between items-start mb-2">
        <div className="flex-1">
          <h3 className="font-semibold text-lg text-gray-900">{candidacy.title}</h3>
          <p className="text-gray-600">{candidacy.company}</p>
        </div>
        <div className="flex gap-1">
          <Button
            variant="ghost"
            size="sm"
            onClick={(e) => {
              e.stopPropagation()
              onEdit(candidacy)
            }}
          >
            <Edit className="h-4 w-4" />
          </Button>
          <Button
            variant="ghost"
            size="sm"
            onClick={handleDelete}
            className="text-red-600 hover:text-red-700"
          >
            <Trash2 className="h-4 w-4" />
          </Button>
        </div>
      </div>

      <div className="flex items-center text-sm text-gray-500 mb-2">
        <Calendar className="h-4 w-4 mr-1" />
        <span>Applied: {formatDate(candidacy.dateApplied)}</span>
      </div>

      {candidacy.jobLink && (
        <div className="mb-2">
          <a
            href={candidacy.jobLink}
            target="_blank"
            rel="noopener noreferrer"
            className="text-blue-600 hover:text-blue-800 text-sm flex items-center"
            onClick={(e) => e.stopPropagation()}
          >
            <ExternalLink className="h-3 w-3 mr-1" />
            View Job Posting
          </a>
        </div>
      )}

      {candidacy.applicationSteps.length > 0 && (
        <div className="mt-3">
          <button
            onClick={(e) => {
              e.stopPropagation()
              setIsExpanded(!isExpanded)
            }}
            className="flex items-center text-sm text-gray-600 hover:text-gray-800"
          >
            Application Steps ({candidacy.applicationSteps.length})
            {isExpanded ? (
              <ChevronUp className="h-4 w-4 ml-1" />
            ) : (
              <ChevronDown className="h-4 w-4 ml-1" />
            )}
          </button>
          
          {isExpanded && (
            <div className="mt-2 space-y-2">
              {candidacy.applicationSteps.map((step, index) => (
                <div key={index} className="text-sm bg-gray-50 p-2 rounded">
                  <div className="font-medium">{step.type}</div>
                  <div className="text-gray-500">{formatDate(step.date)}</div>
                  {step.notes && (
                    <div className="text-gray-600 mt-1">{step.notes}</div>
                  )}
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {candidacy.jobDescription && (
        <div className="mt-2">
          <p className="text-sm text-gray-600 line-clamp-2">
            {candidacy.jobDescription}
          </p>
        </div>
      )}
    </div>
  )
}
