<template>
    <h1>Admin</h1>

    <h3 v-if="subscribers.length > 0">Registrierungen</h3>
    <p v-if="subscribers.length > 0" class="text-muted">Insgesamt {{ subscribers.length }}</p>
    <table v-if="subscribers.length > 0" class="table table-responsive">
        <thead>
            <tr>
                <th scope="col">Vorname</th>
                <th scope="col">Nachname</th>
                <th scope="col">E-Mail-Adresse</th>
                <th scope="col">Aktionen</th>
            </tr>
        </thead>
        <tbody>
            <!--<tr @if (subscriber.DeletionTime != default) { Write(new HtmlString("style=\"text-decoration: line-through;\"")); }>-->
            <tr v-for="subscriber in subscribers" :key="subscriber.id" :class="{ deleted: subscriber.deletionTime !== null }">
                <td>{{ subscriber.givenName }}</td>
                <td>{{ subscriber.familyName }}</td>
                <td>{{ subscriber.mailAddress }}</td>
                <td>
                    <div v-if="subscriber.deletionTime === null" class="form-inline">
                        <button v-if="subscriber.confirmationTime === null" type="button" @click="accept(subscriber)"
                            class="btn btn-outline-success mr-1"><font-awesome-icon icon="fa-check" /></button>
                        <button type="button" @click="reject(subscriber)"
                            class="btn btn-outline-danger"><font-awesome-icon icon="fa-xmark" /></button>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>
    <p v-if="subscribers.length == 0">Keine Registrierungen.</p>
</template>

<script lang="ts">
import { defineComponent, onMounted, ref } from 'vue'
import { Subscriber, getSubscribers, editSubscriber } from '@/api/admin'

export default defineComponent({
  setup () {
    const subscribers = ref<Subscriber[]>([])

    onMounted(async () => {
      subscribers.value = await getSubscribers()
    })

    async function accept (subscriber: Subscriber) {
      subscriber.confirmationTime = new Date()
      await editSubscriber(subscriber)
    }

    async function reject (subscriber: Subscriber) {
      subscriber.deletionTime = new Date()
      await editSubscriber(subscriber)
    }

    return { subscribers, accept, reject }
  }
})
</script>
