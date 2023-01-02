import client, { get } from './client'

export interface Subscriber {
    id: number;
    mailAddress: string | null;
    givenName: string
    familyName: string
    confirmationTime: Date | null;
    deletionTime: Date | null;
}

export function getSubscribers (): Promise<Subscriber[]> {
  return get('/api/admin/subscribers')
}

export async function editSubscriber (subscriber: Subscriber): Promise<boolean> {
  const response = await client.put('/api/admin/subscriber/' + subscriber.id, subscriber)
  return response.status === 204
}
