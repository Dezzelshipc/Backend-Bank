# API for banks-project

## API Endpoints

### Accounts

- /api/v1/organisation/registration
```
in: {login, password, orgName, legalAddress, genDirector, foundingDate}
out: {access token, refresh token}
```

- /api/v1/organisation/authorization
```
in: {login, password}
out: {access token, refresh token}
```

- /api/v1/organisation/removeOrganisation
```
in: {login, password}
out: {}
```

- /api/v1/organisation/getPersonalData
```
in: {access token}
out: {orgName, legalAddress, genDirector, foundingDate}
```

- /api/v1/organisation/changePersonalData
```
in: {access token, orgName?, legalAddress?, genDirector?, foundingDate?}
out: {error, isSuccess}
```

### Branches

- /api/v1/organisation/addBranch
```
in: {access token, branchName, branchAddress, phoneNumber}
out: {branchId, error, isSuccess}
```

- /api/v1/organisation/removeBranch
```
in: {access token, branchId}
out: {error, isSuccess}
```

- /api/v1/organisation/getBranches
```
in: {access token}
out: [list of branches]
```

### Services

- /api/v1/organisation/addService
```
in: {access token, serviceName, description, percent, minLoanPeriod, maxLoanPeriod, isOnline}
out: {serviceId, error, isSuccess}
```

- /api/v1/organisation/removeService
```
in: {access token, serviceId}
out: {error, isSuccess}
```

- /api/v1/organisation/getServices
```
in: {access token}
out: [list of services]
```

## Tokens

- /api/v1/token/refresh_token
```
in: {refresh token}
out: {access token}
```