<template>
  <header>
    <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
      <div class="container">
        <router-link class="navbar-brand" to="/">TravelBlog</router-link>
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target=".navbar-collapse"
          aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="navbar-collapse collapse">
          <ul class="navbar-nav flex-grow-1">
            <li class="nav-item">
              <router-link class="nav-link text-dark" to="/">Home</router-link>
            </li>
            <!--@if (Context.User.IsInRole(Constants.SubscriberRole) || Context.User.IsInRole(Constants.AdminRole))-->
            <!--{-->
            <li class="nav-item">
              <router-link class="nav-link text-dark" to="/posts">Posts</router-link>
            </li>
            <!--}-->
            <li class="nav-item">
              <router-link class="nav-link text-dark" to="/register">Registrieren</router-link>
            </li>
            <!--@if (Context.User.IsInRole(Constants.AdminRole))-->
            <!--{-->
            <li class="nav-item">
              <router-link class="nav-link text-dark" to="/media">Medien</router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link text-dark" to="/admin">Admin</router-link>
            </li>
            <!--}-->
          </ul>
          <!--@if (Context.User.IsInRole(Constants.AdminRole))-->
          <!--{-->
          <form action="~/admin/logout" method="post" class="form-inline my-2 my-lg-0">
            <button type="submit" class="btn btn-outline-secondary my-2 my-sm-0">Abmelden</button>
          </form>
          <!--}-->
        </div>
      </div>
    </nav>
  </header>
  <div class="container">
    <partial name="_CookieConsentPartial" />
    <main role="main" class="pb-3">
      <router-view />
    </main>
  </div>

  <footer class="border-top footer text-muted">
    <div class="container">
      &copy; 2019-2022 Daniel Lerch <small>mit Icons von <a href="https://icons8.com">Icons8</a></small>
    </div>
  </footer>
</template>
