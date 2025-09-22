export enum CandidacyStatus {
  ToApply = 0,
  Applied = 1,
  PreInterview = 2,
  PostInterview = 3,
  Offered = 4,
  Rejected = 5,
  Ghosted = 6,
  Withdrawn = 7
}

export interface CandidacyStep {
  type: string
  date: number // Unix timestamp
  notes?: string
}

export interface CandidacyStepForm {
  type: string
  date: Date // Date object for form handling
  notes?: string
}

export interface Candidacy {
  id: string
  company: string
  title: string
  jobLink?: string
  jobDescription?: string
  dateApplied: number // Unix timestamp
  status: CandidacyStatus
  applicationSteps: CandidacyStep[]
}

export interface CreateCandidacyRequest {
  company: string
  title: string
  jobLink?: string
  jobDescription?: string
  dateApplied: number
  status: CandidacyStatus
  applicationSteps: CandidacyStep[]
}

export interface UpdateCandidacyRequest extends Candidacy {}

export interface UpdateCandidacyStatusRequest {
  id: string
  status: CandidacyStatus
}
