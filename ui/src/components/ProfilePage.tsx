import { useEffect } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm, useFieldArray } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Button } from './ui/button'
import { Input } from './ui/input'
import { Textarea } from './ui/textarea'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select'
import { Label } from './ui/label'
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from './ui/form'
import { Plus, Trash2 } from 'lucide-react'

// Type definitions based on C# models
interface Profile {
  firstName?: string
  lastName?: string
  email?: string
  phoneNumber?: string
  skills?: string[]
  certifications?: Certification[]
  jobExperiences?: JobExperience[]
  otherExperiences?: OtherExperience[]
  education?: Education[]
}

interface Certification {
  name?: string
  earnedDate?: number
  description?: string
  link?: string
}

interface JobExperience {
  title?: string
  company?: string
  location?: string
  startDate?: number
  endDate?: number
  accomplishments?: string[]
}

interface OtherExperience {
  type?: 'Project' | 'VolunteerWork' | 'Other'
  title?: string
  projectOrCompanyName?: string
  startDate?: number
  endDate?: number
  accomplishments?: string[]
}

interface Education {
  awardedBy?: string
  credentialEarned?: string
  endDate?: number
}

interface RPCResult<T> {
  errorMessage?: string
  isError: boolean
  data?: T
}

// Validation schemas
const certificationSchema = z.object({
  name: z.string().optional(),
  earnedDate: z.number().optional(),
  description: z.string().optional(),
  link: z.string().url().optional().or(z.literal('')),
})

const jobExperienceSchema = z.object({
  title: z.string().optional(),
  company: z.string().optional(),
  location: z.string().optional(),
  startDate: z.number().optional(),
  endDate: z.number().optional(),
  accomplishments: z.array(z.string()).optional(),
})

const otherExperienceSchema = z.object({
  type: z.enum(['Project', 'VolunteerWork', 'Other']).optional(),
  title: z.string().optional(),
  projectOrCompanyName: z.string().optional(),
  startDate: z.number().optional(),
  endDate: z.number().optional(),
  accomplishments: z.array(z.string()).optional(),
})

const educationSchema = z.object({
  awardedBy: z.string().optional(),
  credentialEarned: z.string().optional(),
  endDate: z.number().optional(),
})

const profileSchema = z.object({
  firstName: z.string().optional(),
  lastName: z.string().optional(),
  email: z.string().email().optional().or(z.literal('')),
  phoneNumber: z.string().optional(),
  skills: z.array(z.string()).optional(),
  certifications: z.array(certificationSchema).optional(),
  jobExperiences: z.array(jobExperienceSchema).optional(),
  otherExperiences: z.array(otherExperienceSchema).optional(),
  education: z.array(educationSchema).optional(),
})

// RPC functions
const getProfile = async (): Promise<RPCResult<Profile>> => {
  if (typeof window !== 'undefined' && (window as any).GRIND_BREAKER_GetProfile) {
    const result = await (window as any).GRIND_BREAKER_GetProfile()
    console.log(result)
    return result
  }
  throw new Error('RPC function not available')
}

const saveProfile = async (profile: Profile): Promise<string> => {
  if (typeof window !== 'undefined' && (window as any).GRIND_BREAKER_SaveProfile) {
    const result = await (window as any).GRIND_BREAKER_SaveProfile(profile)
    return result
  }
  throw new Error('RPC function not available')
}

export function ProfilePage() {
  const queryClient = useQueryClient()

  // Fetch profile data
  const { data, isLoading, error } = useQuery({
    queryKey: ['profile'],
    queryFn: getProfile,
    retry: false,
  })

  // Save profile mutation
  const saveProfileMutation = useMutation({
    mutationFn: saveProfile,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['profile'] })
    },
  })

  const form = useForm<Profile>({
    resolver: zodResolver(profileSchema),
    defaultValues: {
      firstName: '',
      lastName: '',
      email: '',
      phoneNumber: '',
      skills: [],
      certifications: [],
      jobExperiences: [],
      otherExperiences: [],
      education: [],
    },
  })

  const { fields: skillFields, append: appendSkill, remove: removeSkill } = useFieldArray({
    control: form.control,
    name: 'skills' as any,
  })

  const { fields: certFields, append: appendCert, remove: removeCert } = useFieldArray({
    control: form.control,
    name: 'certifications',
  })

  const { fields: jobFields, append: appendJob, remove: removeJob } = useFieldArray({
    control: form.control,
    name: 'jobExperiences',
  })

  const { fields: otherExpFields, append: appendOtherExp, remove: removeOtherExp } = useFieldArray({
    control: form.control,
    name: 'otherExperiences',
  })

  const { fields: eduFields, append: appendEdu, remove: removeEdu } = useFieldArray({
    control: form.control,
    name: 'education',
  })

  // Update form when profile data loads
  useEffect(() => {
    if (data && data.data) {
      form.reset({
        firstName: data.data.firstName || '',
        lastName: data.data.lastName || '',
        email: data.data.email || '',
        phoneNumber: data.data.phoneNumber || '',
        skills: data.data.skills || [],
        certifications: data.data.certifications || [],
        jobExperiences: data.data.jobExperiences || [],
        otherExperiences: data.data.otherExperiences || [],
        education: data.data.education || [],
      })
    }
  }, [data, form])

  const onSubmit = async (data: Profile) => {
    try {
      await saveProfileMutation.mutateAsync(data)
    } catch (error) {
      console.error('Failed to save profile:', error)
    }
  }

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-600">Loading profile...</p>
        </div>
      </div>
    )
  }

  if (error) {
    console.error('Error loading profile:', error)
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <p className="text-red-600 mb-4">Error loading profile: {error.message}</p>
          <Button onClick={() => window.location.reload()}>Retry</Button>
        </div>
      </div>
    )
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="bg-white shadow rounded-lg">
          <div className="px-4 py-5 sm:p-6">
            <h1 className="text-2xl font-bold text-gray-900 mb-6">Profile</h1>
            
            <Form {...form}>
              <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-8">
                {/* Basic Information */}
                <div className="space-y-6">
                  <h2 className="text-lg font-medium text-gray-900">Basic Information</h2>
                  <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
                    <FormField
                      control={form.control}
                      name="firstName"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>First Name</FormLabel>
                          <FormControl>
                            <Input placeholder="Enter your first name" {...field} />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name="lastName"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Last Name</FormLabel>
                          <FormControl>
                            <Input placeholder="Enter your last name" {...field} />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name="email"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Email</FormLabel>
                          <FormControl>
                            <Input type="email" placeholder="Enter your email" {...field} />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name="phoneNumber"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Phone Number</FormLabel>
                          <FormControl>
                            <Input placeholder="Enter your phone number" {...field} />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>
                </div>

                {/* Skills */}
                <div className="space-y-6">
                  <h2 className="text-lg font-medium text-gray-900">Skills</h2>
                  <div className="space-y-2">
                    {skillFields.map((field, index) => (
                      <div key={field.id} className="flex gap-2">
                        <FormField
                          control={form.control}
                          name={`skills.${index}`}
                          render={({ field }) => (
                            <FormItem className="flex-1">
                              <FormControl>
                                <Input placeholder="Enter a skill" {...field} />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <Button
                          type="button"
                          variant="outline"
                          size="icon"
                          onClick={() => removeSkill(index)}
                        >
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      </div>
                    ))}
                    <Button
                      type="button"
                      variant="outline"
                      onClick={() => appendSkill('' as any)}
                      className="w-full"
                    >
                      <Plus className="h-4 w-4 mr-2" />
                      Add Skill
                    </Button>
                  </div>
                </div>

                {/* Certifications */}
                <div className="space-y-6">
                  <h2 className="text-lg font-medium text-gray-900">Certifications</h2>
                  <div className="space-y-4">
                    {certFields.map((field, index) => (
                      <div key={field.id} className="border rounded-lg p-4 space-y-4">
                        <div className="flex justify-between items-center">
                          <h3 className="font-medium">Certification {index + 1}</h3>
                          <Button
                            type="button"
                            variant="outline"
                            size="icon"
                            onClick={() => removeCert(index)}
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                          <FormField
                            control={form.control}
                            name={`certifications.${index}.name`}
                            render={({ field }) => (
                              <FormItem>
                                <FormLabel>Name</FormLabel>
                                <FormControl>
                                  <Input placeholder="Certification name" {...field} />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                          <FormField
                            control={form.control}
                            name={`certifications.${index}.earnedDate`}
                            render={({ field }) => (
                              <FormItem>
                                <FormLabel>Earned Date</FormLabel>
                                <FormControl>
                                  <Input
                                    type="number"
                                    placeholder="Year (e.g., 2023)"
                                    {...field}
                                    onChange={(e) => field.onChange(parseInt(e.target.value) || undefined)}
                                  />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                          <FormField
                            control={form.control}
                            name={`certifications.${index}.description`}
                            render={({ field }) => (
                              <FormItem className="sm:col-span-2">
                                <FormLabel>Description</FormLabel>
                                <FormControl>
                                  <Textarea placeholder="Certification description" {...field} />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                          <FormField
                            control={form.control}
                            name={`certifications.${index}.link`}
                            render={({ field }) => (
                              <FormItem className="sm:col-span-2">
                                <FormLabel>Link</FormLabel>
                                <FormControl>
                                  <Input placeholder="https://example.com/certificate" {...field} />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                        </div>
                      </div>
                    ))}
                    <Button
                      type="button"
                      variant="outline"
                      onClick={() => appendCert({})}
                      className="w-full"
                    >
                      <Plus className="h-4 w-4 mr-2" />
                      Add Certification
                    </Button>
                  </div>
                </div>

                {/* Job Experiences */}
                <div className="space-y-6">
                  <h2 className="text-lg font-medium text-gray-900">Job Experience</h2>
                  <div className="space-y-4">
                    {jobFields.map((field, index) => (
                      <div key={field.id} className="border rounded-lg p-4 space-y-4">
                        <div className="flex justify-between items-center">
                          <h3 className="font-medium">Job Experience {index + 1}</h3>
                          <Button
                            type="button"
                            variant="outline"
                            size="icon"
                            onClick={() => removeJob(index)}
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                          <FormField
                            control={form.control}
                            name={`jobExperiences.${index}.title`}
                            render={({ field }) => (
                              <FormItem>
                                <FormLabel>Title</FormLabel>
                                <FormControl>
                                  <Input placeholder="Job title" {...field} />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                          <FormField
                            control={form.control}
                            name={`jobExperiences.${index}.company`}
                            render={({ field }) => (
                              <FormItem>
                                <FormLabel>Company</FormLabel>
                                <FormControl>
                                  <Input placeholder="Company name" {...field} />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                          <FormField
                            control={form.control}
                            name={`jobExperiences.${index}.location`}
                            render={({ field }) => (
                              <FormItem>
                                <FormLabel>Location</FormLabel>
                                <FormControl>
                                  <Input placeholder="City, State" {...field} />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                          <FormField
                            control={form.control}
                            name={`jobExperiences.${index}.startDate`}
                            render={({ field }) => (
                              <FormItem>
                                <FormLabel>Start Date</FormLabel>
                                <FormControl>
                                  <Input
                                    type="number"
                                    placeholder="Year (e.g., 2020)"
                                    {...field}
                                    onChange={(e) => field.onChange(parseInt(e.target.value) || undefined)}
                                  />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                          <FormField
                            control={form.control}
                            name={`jobExperiences.${index}.endDate`}
                            render={({ field }) => (
                              <FormItem>
                                <FormLabel>End Date</FormLabel>
                                <FormControl>
                                  <Input
                                    type="number"
                                    placeholder="Year (e.g., 2023)"
                                    {...field}
                                    onChange={(e) => field.onChange(parseInt(e.target.value) || undefined)}
                                  />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                        </div>
                        <div>
                          <Label>Accomplishments</Label>
                          <div className="space-y-2 mt-2">
                            {(form.watch(`jobExperiences.${index}.accomplishments`) || []).map((_, accIndex) => (
                              <div key={accIndex} className="flex gap-2">
                                <FormField
                                  control={form.control}
                                  name={`jobExperiences.${index}.accomplishments.${accIndex}`}
                                  render={({ field }) => (
                                    <FormItem className="flex-1">
                                      <FormControl>
                                        <Input placeholder="Describe an accomplishment" {...field} />
                                      </FormControl>
                                      <FormMessage />
                                    </FormItem>
                                  )}
                                />
                                <Button
                                  type="button"
                                  variant="outline"
                                  size="icon"
                                  onClick={() => {
                                    const currentAccomplishments = form.getValues(`jobExperiences.${index}.accomplishments`) || []
                                    const newAccomplishments = currentAccomplishments.filter((_, i) => i !== accIndex)
                                    form.setValue(`jobExperiences.${index}.accomplishments`, newAccomplishments)
                                  }}
                                >
                                  <Trash2 className="h-4 w-4" />
                                </Button>
                              </div>
                            ))}
                            <Button
                              type="button"
                              variant="outline"
                              onClick={() => {
                                const currentAccomplishments = form.getValues(`jobExperiences.${index}.accomplishments`) || []
                                form.setValue(`jobExperiences.${index}.accomplishments`, [...currentAccomplishments, ''])
                              }}
                              className="w-full"
                            >
                              <Plus className="h-4 w-4 mr-2" />
                              Add Accomplishment
                            </Button>
                          </div>
                        </div>
                      </div>
                    ))}
                    <Button
                      type="button"
                      variant="outline"
                      onClick={() => appendJob({})}
                      className="w-full"
                    >
                      <Plus className="h-4 w-4 mr-2" />
                      Add Job Experience
                    </Button>
                  </div>
                </div>

                {/* Other Experiences */}
                <div className="space-y-6">
                  <h2 className="text-lg font-medium text-gray-900">Other Experience</h2>
                  <div className="space-y-4">
                    {otherExpFields.map((field, index) => (
                      <div key={field.id} className="border rounded-lg p-4 space-y-4">
                        <div className="flex justify-between items-center">
                          <h3 className="font-medium">Other Experience {index + 1}</h3>
                          <Button
                            type="button"
                            variant="outline"
                            size="icon"
                            onClick={() => removeOtherExp(index)}
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                          <FormField
                            control={form.control}
                            name={`otherExperiences.${index}.type`}
                            render={({ field }) => (
                              <FormItem>
                                <FormLabel>Type</FormLabel>
                                <Select onValueChange={field.onChange} value={field.value}>
                                  <FormControl>
                                    <SelectTrigger>
                                      <SelectValue placeholder="Select type" />
                                    </SelectTrigger>
                                  </FormControl>
                                  <SelectContent>
                                    <SelectItem value="Project">Project</SelectItem>
                                    <SelectItem value="VolunteerWork">Volunteer Work</SelectItem>
                                    <SelectItem value="Other">Other</SelectItem>
                                  </SelectContent>
                                </Select>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                          <FormField
                            control={form.control}
                            name={`otherExperiences.${index}.title`}
                            render={({ field }) => (
                              <FormItem>
                                <FormLabel>Title</FormLabel>
                                <FormControl>
                                  <Input placeholder="Experience title" {...field} />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                          <FormField
                            control={form.control}
                            name={`otherExperiences.${index}.projectOrCompanyName`}
                            render={({ field }) => (
                              <FormItem>
                                <FormLabel>Project/Company Name</FormLabel>
                                <FormControl>
                                  <Input placeholder="Project or company name" {...field} />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                          <FormField
                            control={form.control}
                            name={`otherExperiences.${index}.startDate`}
                            render={({ field }) => (
                              <FormItem>
                                <FormLabel>Start Date</FormLabel>
                                <FormControl>
                                  <Input
                                    type="number"
                                    placeholder="Year (e.g., 2020)"
                                    {...field}
                                    onChange={(e) => field.onChange(parseInt(e.target.value) || undefined)}
                                  />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                          <FormField
                            control={form.control}
                            name={`otherExperiences.${index}.endDate`}
                            render={({ field }) => (
                              <FormItem>
                                <FormLabel>End Date</FormLabel>
                                <FormControl>
                                  <Input
                                    type="number"
                                    placeholder="Year (e.g., 2023)"
                                    {...field}
                                    onChange={(e) => field.onChange(parseInt(e.target.value) || undefined)}
                                  />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                        </div>
                        <div>
                          <Label>Accomplishments</Label>
                          <div className="space-y-2 mt-2">
                            {(form.watch(`otherExperiences.${index}.accomplishments`) || []).map((_, accIndex) => (
                              <div key={accIndex} className="flex gap-2">
                                <FormField
                                  control={form.control}
                                  name={`otherExperiences.${index}.accomplishments.${accIndex}`}
                                  render={({ field }) => (
                                    <FormItem className="flex-1">
                                      <FormControl>
                                        <Input placeholder="Describe an accomplishment" {...field} />
                                      </FormControl>
                                      <FormMessage />
                                    </FormItem>
                                  )}
                                />
                                <Button
                                  type="button"
                                  variant="outline"
                                  size="icon"
                                  onClick={() => {
                                    const currentAccomplishments = form.getValues(`otherExperiences.${index}.accomplishments`) || []
                                    const newAccomplishments = currentAccomplishments.filter((_, i) => i !== accIndex)
                                    form.setValue(`otherExperiences.${index}.accomplishments`, newAccomplishments)
                                  }}
                                >
                                  <Trash2 className="h-4 w-4" />
                                </Button>
                              </div>
                            ))}
                            <Button
                              type="button"
                              variant="outline"
                              onClick={() => {
                                const currentAccomplishments = form.getValues(`otherExperiences.${index}.accomplishments`) || []
                                form.setValue(`otherExperiences.${index}.accomplishments`, [...currentAccomplishments, ''])
                              }}
                              className="w-full"
                            >
                              <Plus className="h-4 w-4 mr-2" />
                              Add Accomplishment
                            </Button>
                          </div>
                        </div>
                      </div>
                    ))}
                    <Button
                      type="button"
                      variant="outline"
                      onClick={() => appendOtherExp({})}
                      className="w-full"
                    >
                      <Plus className="h-4 w-4 mr-2" />
                      Add Other Experience
                    </Button>
                  </div>
                </div>

                {/* Education */}
                <div className="space-y-6">
                  <h2 className="text-lg font-medium text-gray-900">Education</h2>
                  <div className="space-y-4">
                    {eduFields.map((field, index) => (
                      <div key={field.id} className="border rounded-lg p-4 space-y-4">
                        <div className="flex justify-between items-center">
                          <h3 className="font-medium">Education {index + 1}</h3>
                          <Button
                            type="button"
                            variant="outline"
                            size="icon"
                            onClick={() => removeEdu(index)}
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                          <FormField
                            control={form.control}
                            name={`education.${index}.awardedBy`}
                            render={({ field }) => (
                              <FormItem>
                                <FormLabel>Awarded By</FormLabel>
                                <FormControl>
                                  <Input placeholder="Institution name" {...field} />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                          <FormField
                            control={form.control}
                            name={`education.${index}.credentialEarned`}
                            render={({ field }) => (
                              <FormItem>
                                <FormLabel>Credential Earned</FormLabel>
                                <FormControl>
                                  <Input placeholder="Degree or certificate" {...field} />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                          <FormField
                            control={form.control}
                            name={`education.${index}.endDate`}
                            render={({ field }) => (
                              <FormItem>
                                <FormLabel>End Date</FormLabel>
                                <FormControl>
                                  <Input
                                    type="number"
                                    placeholder="Year (e.g., 2023)"
                                    {...field}
                                    onChange={(e) => field.onChange(parseInt(e.target.value) || undefined)}
                                  />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                        </div>
                      </div>
                    ))}
                    <Button
                      type="button"
                      variant="outline"
                      onClick={() => appendEdu({})}
                      className="w-full"
                    >
                      <Plus className="h-4 w-4 mr-2" />
                      Add Education
                    </Button>
                  </div>
                </div>

                {/* Submit Button */}
                <div className="flex justify-end">
                  <Button
                    type="submit"
                    disabled={saveProfileMutation.isPending}
                    className="px-8"
                  >
                    {saveProfileMutation.isPending ? 'Saving...' : 'Save Profile'}
                  </Button>
                </div>
              </form>
            </Form>
          </div>
        </div>
      </div>
    </div>
  )
}