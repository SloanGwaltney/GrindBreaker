import { describe, it, expect, beforeEach, vi } from 'vitest'
import { screen, waitFor, fireEvent } from '@testing-library/react'
import { render, mockGetProfile, mockSaveProfile } from '../../test-utils'
import { ProfilePage } from '../ProfilePage'

describe('ProfilePage - Basic Tests', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Component Rendering', () => {
    it('should render loading state', async () => {
      mockGetProfile.mockImplementation(() => new Promise(() => {}))
      
      render(<ProfilePage />)
      
      expect(screen.getByText('Loading profile...')).toBeInTheDocument()
    })

    it('should render error state', async () => {
      const errorMessage = 'Failed to fetch profile'
      mockGetProfile.mockRejectedValue(new Error(errorMessage))
      
      render(<ProfilePage />)
      
      await waitFor(() => {
        expect(screen.getByText(`Error loading profile: ${errorMessage}`)).toBeInTheDocument()
      })
      
      expect(screen.getByRole('button', { name: 'Retry' })).toBeInTheDocument()
    })

    it('should render form when data loads successfully', async () => {
      mockGetProfile.mockResolvedValue({
        isError: false,
        data: {},
      })

      render(<ProfilePage />)

      await waitFor(() => {
        expect(screen.getByText('Profile')).toBeInTheDocument()
        expect(screen.getByText('Basic Information')).toBeInTheDocument()
        expect(screen.getByLabelText('First Name')).toBeInTheDocument()
        expect(screen.getByLabelText('Last Name')).toBeInTheDocument()
        expect(screen.getByLabelText('Email')).toBeInTheDocument()
        expect(screen.getByLabelText('Phone Number')).toBeInTheDocument()
      })
    })
  })

  describe('Form Interactions', () => {
    beforeEach(async () => {
      mockGetProfile.mockResolvedValue({
        isError: false,
        data: {},
      })
    })

    it('should allow typing in form fields', async () => {
      render(<ProfilePage />)

      await waitFor(() => {
        expect(screen.getByLabelText('First Name')).toBeInTheDocument()
      })

      const firstNameInput = screen.getByLabelText('First Name')
      fireEvent.change(firstNameInput, { target: { value: 'John' } })

      expect(firstNameInput).toHaveValue('John')
    })

    it('should submit form with data', async () => {
      mockSaveProfile.mockResolvedValue('Profile saved successfully')

      render(<ProfilePage />)

      await waitFor(() => {
        expect(screen.getByLabelText('First Name')).toBeInTheDocument()
      })

      // Fill in form data
      fireEvent.change(screen.getByLabelText('First Name'), { target: { value: 'John' } })
      fireEvent.change(screen.getByLabelText('Last Name'), { target: { value: 'Doe' } })
      fireEvent.change(screen.getByLabelText('Email'), { target: { value: 'john@example.com' } })

      // Submit form
      fireEvent.click(screen.getByRole('button', { name: 'Save Profile' }))

      await waitFor(() => {
        expect(mockSaveProfile).toHaveBeenCalledWith({
          firstName: 'John',
          lastName: 'Doe',
          email: 'john@example.com',
          phoneNumber: '',
          skills: [],
          certifications: [],
          jobExperiences: [],
          otherExperiences: [],
          education: [],
        })
      })
    })

    it('should show loading state during form submission', async () => {
      mockSaveProfile.mockImplementation(() => new Promise(() => {})) // Never resolves

      render(<ProfilePage />)

      await waitFor(() => {
        expect(screen.getByRole('button', { name: 'Save Profile' })).toBeInTheDocument()
      })

      // Submit form
      fireEvent.click(screen.getByRole('button', { name: 'Save Profile' }))

      await waitFor(() => {
        expect(screen.getByRole('button', { name: 'Saving...' })).toBeInTheDocument()
        expect(screen.getByRole('button', { name: 'Saving...' })).toBeDisabled()
      })
    })
  })

  describe('Data Population', () => {
    it('should populate form with provided data', async () => {
      const mockProfile = {
        firstName: 'Jane',
        lastName: 'Smith',
        email: 'jane.smith@example.com',
        phoneNumber: '555-123-4567',
        skills: ['React', 'TypeScript'],
        certifications: [],
        jobExperiences: [],
        otherExperiences: [],
        education: [],
      }

      mockGetProfile.mockResolvedValue({
        isError: false,
        data: mockProfile,
      })

      render(<ProfilePage />)

      await waitFor(() => {
        expect(screen.getByDisplayValue('Jane')).toBeInTheDocument()
        expect(screen.getByDisplayValue('Smith')).toBeInTheDocument()
        expect(screen.getByDisplayValue('jane.smith@example.com')).toBeInTheDocument()
        expect(screen.getByDisplayValue('555-123-4567')).toBeInTheDocument()
      })
    })
  })
})
