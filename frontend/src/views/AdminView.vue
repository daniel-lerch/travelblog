<template>
    <h1>Admin</h1>

    @switch (Model.Status)
    {
    case "success":
    <div class="alert alert-success alert-dismissible fade show" role="alert">
        Erfolgreich bestätigt/gelöscht
        <button type="button" class="close" data-dismiss="alert" aria-label="Close">
            <span aria-hidden="true">&times;</span>
        </button>
    </div>
    break;
    case "error":
    <div class="alert alert-danger alert-dismissible fade show" role="alert">
        Bestätigen/Löschen fehlgeschlagen
        <button type="button" class="close" data-dismiss="alert" aria-label="Close">
            <span aria-hidden="true">&times;</span>
        </button>
    </div>
    break;
    }

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
            <tr v-for="subscriber in subscribers" :key="subscriber.id">
                <td>{{ subscriber.givenName }}</td>
                <td>{{ subscriber.familyName }}</td>
                <td>{{ subscriber.mailAddress }}</td>
                <td>
                    @if (subscriber.DeletionTime == default)
                    {
                    <form action="" method="post" class="form-inline">
                        <button type="submit" formaction="~/admin/confirm?id=@subscriber.Id"
                            class="btn btn-outline-success mr-1">✔️</button>
                        <button type="submit" formaction="~/admin/delete?id=@subscriber.Id"
                            class="btn btn-outline-danger">❌</button>
                        <button type="submit" formaction="~/admin/delete?id=@subscriber.Id"
                            class="btn btn-outline-danger mr-1">❌</button>
                    </form>
                    }
                </td>
            </tr>
        </tbody>
    </table>
    <p v-if="subscribers.length == 0">Keine Registrierungen.</p>
</template>

<script lang="ts">
import { defineComponent, onMounted, ref, computed } from 'vue'
import { Subscriber, getSubscribers } from '@/api/admin'

export default defineComponent({
  setup () {
    const subscribers = ref<Subscriber[]>([])

    onMounted(async () => {
      subscribers.value = await getSubscribers()
    })

    return { subscribers }
  }
})
</script>
