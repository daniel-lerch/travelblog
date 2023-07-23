import client from './client'

export interface SubscribeRequest {
    mailAddress: string;
    givenName: string;
    familyName: string;
    comment: string;
}

export async function subscribe (request: SubscribeRequest): Promise<boolean> {
  try {
  const response = await client.post('/api/subscribe', request)
  return response.status === 204
  } catch (error) {
    return false
  }
}
