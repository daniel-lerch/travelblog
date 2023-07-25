import client from './client'

export interface SubscribeRequest {
  mailAddress: string;
  givenName: string;
  familyName: string;
  comment: string;
}

export interface ProfileResponse {
  mailAddress: string;
  givenName: string;
  familyName: string;
}

export async function subscribe (request: SubscribeRequest): Promise<boolean | undefined> {
  try {
    const response = await client.post('/api/subscribe', request)
    if (response.status === 204) return true
    if (response.status === 409) return false
    console.warn('Unexpected status from /api/subscribe: ' + response.status)
    return undefined
  } catch (error) {
    return undefined
  }
}

export async function profile (token: string): Promise<ProfileResponse | null | undefined> {
  try {
    const response = await client.get('/api/profile?token=' + token)
    if (response.status === 200) return response.data as ProfileResponse
    if (response.status === 404) return null
    console.warn('Unexpected status from /api/profile: ' + response.status)
    return undefined
  } catch (error) {
    return undefined
  }
}

export async function unsubscribe (token: string): Promise<boolean | undefined> {
  try {
    const response = await client.post('/api/unsubscribe?token=' + token)
    if (response.status === 204) return true
    if (response.status === 404) return false
    console.warn('Unexpected status from /api/unsubscribe: ' + response.status)
    return undefined
  } catch (error) {
    return undefined
  }
}
