import { useState, useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Button } from './ui/button'
import { Input } from './ui/input'
import { Label } from './ui/label'
import { Textarea } from './ui/textarea'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select'
import { DatePicker } from './ui/date-picker'
import { X, Plus, Trash2 } from 'lucide-react'
import type { Candidacy, CandidacyStepForm } from '../lib/types'
import { CandidacyStatus } from '../lib/types'

const candidacySchema = z.object({
  company: z.string().min(1, 'Company is required'),
  title: z.string().min(1, 'Title is required'),
  jobLink: z.string().url().optional().or(z.literal('')),
  jobDescription: z.string().optional(),
  dateApplied: z.date({
    required_error: 'Date applied is required',
  }),
  status: z.nativeEnum(CandidacyStatus),
  applicationSteps: z.array(z.object({
    type: z.string().min(1, 'Step type is required'),
    date: z.date({
      required_error: 'Step date is required',
    }),
    notes: z.string().optional(),
  })),
})

type CandidacyFormData = z.infer<typeof candidacySchema>

interface CandidacyFormProps {
  candidacy?: Candidacy
  onSubmit: (data: CandidacyFormData) => void
  onCancel: () => void
  isLoading?: boolean
}

export function CandidacyForm({ candidacy, onSubmit, onCancel, isLoading }: CandidacyFormProps) {
  const [applicationSteps, setApplicationSteps] = useState<CandidacyStepForm[]>(
    candidacy?.applicationSteps?.map(step => ({
      ...step,
      date: new Date(step.date)
    })) || []
  )

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    watch,
  } = useForm<CandidacyFormData>({
    resolver: zodResolver(candidacySchema),
    defaultValues: {
      company: candidacy?.company || '',
      title: candidacy?.title || '',
      jobLink: candidacy?.jobLink || '',
      jobDescription: candidacy?.jobDescription || '',
      dateApplied: candidacy?.dateApplied ? new Date(candidacy.dateApplied) : new Date(),
      status: candidacy?.status || CandidacyStatus.ToApply,
      applicationSteps: candidacy?.applicationSteps?.map(step => ({
        ...step,
        date: new Date(step.date)
      })) || [],
    },
  })

  const watchedStatus = watch('status')

  useEffect(() => {
    setValue('applicationSteps', applicationSteps)
  }, [applicationSteps, setValue])

  const addApplicationStep = () => {
    const newStep = {
      type: '',
      date: new Date(),
      notes: '',
    }
    setApplicationSteps([...applicationSteps, newStep])
  }

  const removeApplicationStep = (index: number) => {
    setApplicationSteps(applicationSteps.filter((_, i) => i !== index))
  }

  const updateApplicationStep = (index: number, field: keyof CandidacyStepForm, value: string | Date) => {
    const updatedSteps = applicationSteps.map((step, i) =>
      i === index ? { ...step, [field]: value } : step
    )
    setApplicationSteps(updatedSteps)
  }

  const statusOptions = [
    { value: CandidacyStatus.ToApply.toString(), label: 'To Apply' },
    { value: CandidacyStatus.Applied.toString(), label: 'Applied' },
    { value: CandidacyStatus.PreInterview.toString(), label: 'Pre-interview' },
    { value: CandidacyStatus.PostInterview.toString(), label: 'Post-interview' },
    { value: CandidacyStatus.Offered.toString(), label: 'Offered' },
    { value: CandidacyStatus.Rejected.toString(), label: 'Rejected' },
    { value: CandidacyStatus.Ghosted.toString(), label: 'Ghosted' },
    { value: CandidacyStatus.Withdrawn.toString(), label: 'Withdrawn' },
  ]

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-lg shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        <div className="flex items-center justify-between p-6 border-b">
          <h2 className="text-xl font-semibold">
            {candidacy ? 'Edit Candidacy' : 'Create New Candidacy'}
          </h2>
          <Button variant="ghost" size="sm" onClick={onCancel}>
            <X className="h-4 w-4" />
          </Button>
        </div>

        <form onSubmit={handleSubmit((data) => {
          console.log('Form submitted with data:', data)
          console.log('Application steps:', applicationSteps)
          const formData = {
            ...data,
            applicationSteps: applicationSteps
          }
          console.log('Final form data:', formData)
          onSubmit(formData)
        }, (errors) => {
          console.log('Form validation errors:', errors)
        })} className="p-6 space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <Label htmlFor="company">Company *</Label>
              <Input
                id="company"
                {...register('company')}
                className={errors.company ? 'border-red-500' : ''}
              />
              {errors.company && (
                <p className="text-red-500 text-sm mt-1">{errors.company.message}</p>
              )}
            </div>

            <div>
              <Label htmlFor="title">Job Title *</Label>
              <Input
                id="title"
                {...register('title')}
                className={errors.title ? 'border-red-500' : ''}
              />
              {errors.title && (
                <p className="text-red-500 text-sm mt-1">{errors.title.message}</p>
              )}
            </div>
          </div>

          <div>
            <Label htmlFor="jobLink">Job Link</Label>
            <Input
              id="jobLink"
              type="url"
              {...register('jobLink')}
              placeholder="https://..."
              className={errors.jobLink ? 'border-red-500' : ''}
            />
            {errors.jobLink && (
              <p className="text-red-500 text-sm mt-1">{errors.jobLink.message}</p>
            )}
          </div>

          <div>
            <Label htmlFor="jobDescription">Job Description</Label>
            <Textarea
              id="jobDescription"
              {...register('jobDescription')}
              rows={3}
              placeholder="Brief description of the role..."
            />
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <Label htmlFor="dateApplied">Date Applied *</Label>
              <DatePicker
                value={watch('dateApplied')}
                onChange={(date) => setValue('dateApplied', date || new Date())}
                placeholder="Select application date"
                className={errors.dateApplied ? 'border-red-500' : ''}
              />
              {errors.dateApplied && (
                <p className="text-red-500 text-sm mt-1">{errors.dateApplied.message}</p>
              )}
            </div>

            <div>
              <Label htmlFor="status">Status *</Label>
              <Select
                value={watchedStatus?.toString()}
                onValueChange={(value) => setValue('status', parseInt(value) as CandidacyStatus)}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select status" />
                </SelectTrigger>
                <SelectContent>
                  {statusOptions.map((option) => (
                    <SelectItem key={option.value} value={option.value}>
                      {option.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          <div>
            <div className="flex items-center justify-between mb-2">
              <Label>Application Steps</Label>
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={addApplicationStep}
              >
                <Plus className="h-4 w-4 mr-1" />
                Add Step
              </Button>
            </div>

            <div className="space-y-2">
              {applicationSteps.map((step, index) => (
                <div key={index} className="border rounded-lg p-3 space-y-2">
                  <div className="flex items-center justify-between">
                    <span className="text-sm font-medium">Step {index + 1}</span>
                    <Button
                      type="button"
                      variant="ghost"
                      size="sm"
                      onClick={() => removeApplicationStep(index)}
                      className="text-red-600 hover:text-red-700"
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-2 gap-2">
                    <div>
                      <Input
                        placeholder="Step type (e.g., Phone Screen, Interview)"
                        value={step.type}
                        onChange={(e) => updateApplicationStep(index, 'type', e.target.value)}
                      />
                    </div>
                    <div>
                      <DatePicker
                        value={step.date}
                        onChange={(date) => updateApplicationStep(index, 'date', date || new Date())}
                        placeholder="Select step date"
                      />
                    </div>
                  </div>

                  <div>
                    <Textarea
                      placeholder="Notes (optional)"
                      value={step.notes || ''}
                      onChange={(e) => updateApplicationStep(index, 'notes', e.target.value)}
                      rows={2}
                    />
                  </div>
                </div>
              ))}
            </div>
          </div>

          <div className="flex justify-end gap-2 pt-4 border-t">
            <Button type="button" variant="outline" onClick={onCancel}>
              Cancel
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading ? 'Saving...' : candidacy ? 'Update' : 'Create'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  )
}
