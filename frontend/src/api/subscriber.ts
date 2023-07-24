import client from './client'

export interface SubscribeRequest {
  mailAddress: string;
  givenName: string;
  familyName: string;
  comment: string;
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
