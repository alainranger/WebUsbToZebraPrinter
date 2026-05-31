# Branch protection checklist for main

Use this checklist in GitHub repository settings to enforce CI before merge.

## Branch rule target

- Branch name pattern: `main`

## Recommended protections

- [ ] Require a pull request before merging
- [ ] Require approvals: at least `1`
- [ ] Dismiss stale approvals when new commits are pushed
- [ ] Require conversation resolution before merging
- [ ] Require status checks to pass before merging
- [ ] Require branches to be up to date before merging
- [ ] Do not allow bypassing the above settings (except admins if your policy allows it)

## Required status checks

Select these checks as required:

- [ ] `.NET Format`
- [ ] `Web + Markdown Format`
- [ ] `.NET Build + Tests`
- [ ] `Web Compile`

## Optional hardening

- [ ] Require linear history
- [ ] Restrict who can push to matching branches
- [ ] Require signed commits
- [ ] Enable merge queue for busy repositories
