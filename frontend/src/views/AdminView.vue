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

@if (Model.PendingSubscribers.Count > 0)
{
    <h3>Unbestätigte Registrierungen</h3>
    <p class="text-muted">Insgesamt @Model.PendingSubscribers.Count</p>
    <table class="table table-responsive">
        <thead>
            <tr>
                <th scope="col">Vorname</th>
                <th scope="col">Nachname</th>
                <th scope="col">E-Mail-Adresse</th>
                <th scope="col">Aktionen</th>
            </tr>
        </thead>
        <tbody>
            @foreach (Subscriber subscriber in Model.PendingSubscribers)
            {
                <tr>
                    <td>@subscriber.GivenName</td>
                    <td>@subscriber.FamilyName</td>
                    <td>@subscriber.MailAddress</td>
                    <td>
                        <form action="" method="post" class="form-inline">
                            <button type="submit" formaction="~/admin/confirm?id=@subscriber.Id" class="btn btn-outline-success mr-1">✔️</button>
                            <button type="submit" formaction="~/admin/delete?id=@subscriber.Id" class="btn btn-outline-danger">❌</button>
                        </form>
                    </td>
                </tr>
            }
        </tbody>
    </table>
}
else
{
    <p>Keine unbestätigten Registrierungen.</p>
}

@if (Model.ConfirmedSubscribers.Count > 0)
{
    <h3>Bestätigte Registrierungen</h3>
    <p class="text-muted">Insgesamt @Model.ConfirmedSubscribers.Count</p>
    <table class="table table-responsive">
        <thead>
            <tr>
                <th scope="col">Vorname</th>
                <th scope="col">Nachname</th>
                <th scope="col">E-Mail-Adresse</th>
                <th scope="col">Aktionen</th>
            </tr>
        </thead>
        <tbody>
            @foreach (Subscriber subscriber in Model.ConfirmedSubscribers)
            {
                <!--<tr @if (subscriber.DeletionTime != default) { Write(new HtmlString("style=\"text-decoration: line-through;\"")); }>-->
                <tr>
                    <td>@subscriber.GivenName</td>
                    <td>@subscriber.FamilyName</td>
                    <td>@subscriber.MailAddress</td>
                    <td>
                        @if (subscriber.DeletionTime == default)
                        {
                            <form action="" method="post" class="form-inline">
                                <button type="submit" formaction="~/admin/delete?id=@subscriber.Id" class="btn btn-outline-danger mr-1">❌</button>
                                @if (SiteOptions.Value.EnableDebugFeatures)
                                {
                                    <button type="button" class="btn btn-outline-secondary clipboard" data-token="@subscriber.Token">📋</button>
                                }
                            </form>
                        }
                    </td>
                </tr>
            }
        </tbody>
    </table>
}
else
{
    <p>Keine bestätigten Registrierungen.</p>
}
</template>
